using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ClientAsyncNet : BaseManager<ClientAsyncNet>
{
    private Socket socket;

    private byte[] cacheBytes = new byte[1024*1024];
    private int cacheNum = 0;

    public Queue<BaseMsg> receiveQueue = new Queue<BaseMsg>();

    private int SEND_HEART_MSG_TIME = 10;
    private HeartMsg hearMsg = new HeartMsg();

    public ClientAsyncNet()
    {
        ThreadPool.QueueUserWorkItem(SendHeartMsg);
    }

    public void Connect(string ip, int port)
    {
        if (socket != null && socket.Connected)
            return;
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.RemoteEndPoint = ipPoint;
        args.Completed += (lambdaSocket, lambdaArgs) =>
        {
            if (lambdaArgs.SocketError == SocketError.Success)
            {
                Debug.Log("连接服务器成功 我的地址是" 
                    + IPAddress.Parse(((IPEndPoint)this.socket.LocalEndPoint).Address.ToString()) 
                    + ":" + ((IPEndPoint)this.socket.LocalEndPoint).Port.ToString());

                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.SetBuffer(cacheBytes, 0, cacheBytes.Length);
                receiveArgs.Completed += ReceiveCallBack;//消息处理
                //开始异步接收，接收到消息后会执行Completed委托
                (lambdaSocket as Socket).ReceiveAsync(receiveArgs);
            }
            else
            {
                Debug.Log("连接失败" + lambdaArgs.SocketError);
            }
        };

        socket.ConnectAsync(args);//连接
    }

    //收消息完成后进行消息处理的回调函数
    private void ReceiveCallBack(object sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            HandleReceiveMsg(args.BytesTransferred);
            //继续去收消息 设置收消息的buffer
            //cacheNum 缓冲区的起始位置开始的偏移量
            //设置的缓冲区的大小
            args.SetBuffer(cacheNum, args.Buffer.Length - cacheNum);
            //继续异步收消息
            if (this.socket != null && this.socket.Connected)
                socket.ReceiveAsync(args);
            else
                Close();
        }
        else
        {
            Debug.Log("接受消息出错" + args.SocketError);
            Close();
        }
    }

    private void SendHeartMsg(object obj)
    {
        while(true)
        {
            if (this.socket != null && this.socket.Connected)
            {
                Send(hearMsg);
                Thread.Sleep(SEND_HEART_MSG_TIME*1000);
            }
        } 
    }

    public void Send(BaseMsg msg)
    {
        if (this.socket != null && this.socket.Connected)//会自己变为false?
        {
            if(msg is TalkMsg)
            {
                //发消息了
                Debug.Log("发聊天消息了");
            }
            else if(msg is HeartMsg)
            {
                Debug.Log("发心跳包了");
            }
            
            byte[] bytes = msg.Writing();
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.SetBuffer(bytes, 0, bytes.Length);
            args.Completed += (socket, args) =>
            {
                if (args.SocketError != SocketError.Success)
                {
                    Debug.Log("发送消息失败" + args.SocketError);
                    Close();
                }
            };
            this.socket.SendAsync(args);
        }
        else
        {
            Close();
        }
    }

    //客户端发起关闭请求的API
    public void Close()
    {
        if (socket != null)
        {
            QuitMsg msg = new QuitMsg();
            socket.Send(msg.Writing());//发送给服务器 关闭连接消息
            socket.Shutdown(SocketShutdown.Both);
            socket.Disconnect(false);
            socket.Close();
            socket = null;
        }
    }

    private void HandleReceiveMsg(int receiveNum)
    {
        int msgID = 0;
        int msgLength = 0;
        int nowIndex = 0;

        cacheNum += receiveNum;

        //while 会处理所有数据
        //每一次循环处理解析一条消息
        while (true)
        {
            //每次将长度设置为-1 是避免上一次解析的数据 影响这一次的判断
            msgLength = -1;
            if (cacheNum - nowIndex >= 8)
            {
                //解析消息体ID
                msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
                //解析长度
                msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
            }

            if (cacheNum - nowIndex >= msgLength && msgLength != -1)
            {
                //解析消息体 客户端是不会收到quit和heart消息的
                BaseMsg baseMsg = null;
                switch (msgID)
                {
                    case 1004:
                        baseMsg = new BoardMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 1005:
                        baseMsg = new TalkMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 1007:
                        baseMsg=new MatchResultMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 1011:
                        baseMsg = new RegisterResultMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 1013:
                        baseMsg = new LoginResultMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 1015:
                        baseMsg = new UserInfoMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 1016:
                        baseMsg = new UpdateUserInfoMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 1018:
                        baseMsg = new StopStepMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 1019:
                        baseMsg = new GameResultMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                }
                if (baseMsg != null)
                {
                    ThreadPool.QueueUserWorkItem(MsgHandle, baseMsg);
                    //receiveQueue.Enqueue(baseMsg);//压入接收队列
                }
                    
                nowIndex += msgLength;
                if (nowIndex == cacheNum)
                {
                    cacheNum = 0;
                    break;
                }
            }
            else
            {
                if (msgLength != -1)
                    nowIndex -= 8;
                //就是把剩余没有解析的字节数组内容 移到前面来 用于缓存下次继续解析
                Array.Copy(cacheBytes, nowIndex, cacheBytes, 0, cacheNum - nowIndex);
                cacheNum = cacheNum - nowIndex;//cacheNum代表剩下没处理的数据
                break;
            }
        }
    }

    private void MsgHandle(object obj)
    {
        switch (obj)
        {
            case BoardMsg msg:
                BoardMsg boardMsg = obj as BoardMsg;
                GameDataMgr.Instance.SetChessPos(boardMsg.x, boardMsg.y);
                break;
            case MatchResultMsg msg:
                if (msg.result==true)
                {
                    GameDataMgr.Instance.isWhite = msg.isWhite;
                    Debug.Log("匹配成功，开始你的表演");
                    OnlinegameChessBoardMgr.Instance.readytoLoad = true;
                    Debug.Log("匹配成功，开始你的表演");
                }
                break;
            case TalkMsg msg:
                UIMgr.Instance.chatPanel.UpdateText(msg.sentence, msg.username);
                break;
            case RegisterResultMsg msg:
                UIMgr.Instance.GetPanel<RegisterPanel>().TryRegister(msg.result);
                break;
            case LoginResultMsg msg:
                UIMgr.Instance.GetPanel<LoginPanel>().LoginIn(msg.result);
                break;
            case UserInfoMsg msg:
                if(msg.userType==0)
                {
                    //存储我自己的信息到内存中
                    GameDataMgr.Instance.SaveLocalUserInfo(msg.userInfo);
                }
                else if(msg.userType==1)
                {
                    //存储对手信息到内存中
                    GameDataMgr.Instance.SaveOpponentUserInfo(msg.userInfo);            
                }
                break;
            case UpdateUserInfoMsg msg:
                GameDataMgr.Instance.SaveLocalUserInfo(msg.userInfo);
                break;
            case GameResultMsg msg:
                Debug.Log("收到比赛结果");
                GameDataMgr.Instance.gameResult = msg.result;
                break;
            case StopStepMsg msg:
                GameDataMgr.Instance.turn = !GameDataMgr.Instance.turn;
                OnlinechessBoard board = OnlinegameChessBoardMgr.Instance.GetCurrentBoard();
                board.opponentStopStep = true;
                break;
        }
    }
}
