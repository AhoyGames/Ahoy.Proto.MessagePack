﻿/**
 * Autogenerated by Thrift Compiler (0.13.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace HttpServiceData.Leaderboards
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class LeaderboardConsumedCoinsReq : TBase
  {
    private string _user;
    private string _authToken;
    private int _consumedCoins;
    private int _totalCoins;

    public string User
    {
      get
      {
        return _user;
      }
      set
      {
        __isset.user = true;
        this._user = value;
      }
    }

    public string AuthToken
    {
      get
      {
        return _authToken;
      }
      set
      {
        __isset.authToken = true;
        this._authToken = value;
      }
    }

    public int ConsumedCoins
    {
      get
      {
        return _consumedCoins;
      }
      set
      {
        __isset.consumedCoins = true;
        this._consumedCoins = value;
      }
    }

    public int TotalCoins
    {
      get
      {
        return _totalCoins;
      }
      set
      {
        __isset.totalCoins = true;
        this._totalCoins = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool user;
      public bool authToken;
      public bool consumedCoins;
      public bool totalCoins;
    }

    public LeaderboardConsumedCoinsReq() {
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 1:
              if (field.Type == TType.String) {
                User = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.String) {
                AuthToken = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.I32) {
                ConsumedCoins = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 4:
              if (field.Type == TType.I32) {
                TotalCoins = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }
      finally
      {
        iprot.DecrementRecursionDepth();
      }
    }

    public void Write(TProtocol oprot) {
      oprot.IncrementRecursionDepth();
      try
      {
        TStruct struc = new TStruct("LeaderboardConsumedCoinsReq");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (User != null && __isset.user) {
          field.Name = "user";
          field.Type = TType.String;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(User);
          oprot.WriteFieldEnd();
        }
        if (AuthToken != null && __isset.authToken) {
          field.Name = "authToken";
          field.Type = TType.String;
          field.ID = 2;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(AuthToken);
          oprot.WriteFieldEnd();
        }
        if (__isset.consumedCoins) {
          field.Name = "consumedCoins";
          field.Type = TType.I32;
          field.ID = 3;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(ConsumedCoins);
          oprot.WriteFieldEnd();
        }
        if (__isset.totalCoins) {
          field.Name = "totalCoins";
          field.Type = TType.I32;
          field.ID = 4;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(TotalCoins);
          oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    public override string ToString() {
      StringBuilder __sb = new StringBuilder("LeaderboardConsumedCoinsReq(");
      bool __first = true;
      if (User != null && __isset.user) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("User: ");
        __sb.Append(User);
      }
      if (AuthToken != null && __isset.authToken) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("AuthToken: ");
        __sb.Append(AuthToken);
      }
      if (__isset.consumedCoins) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("ConsumedCoins: ");
        __sb.Append(ConsumedCoins);
      }
      if (__isset.totalCoins) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("TotalCoins: ");
        __sb.Append(TotalCoins);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
