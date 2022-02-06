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
  public partial class LeaderboardUserRep : TBase
  {
    private HttpServiceData.UserData.UserRep _user;
    private int _leaderboardScore;

    public HttpServiceData.UserData.UserRep User
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

    public int LeaderboardScore
    {
      get
      {
        return _leaderboardScore;
      }
      set
      {
        __isset.leaderboardScore = true;
        this._leaderboardScore = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool user;
      public bool leaderboardScore;
    }

    public LeaderboardUserRep() {
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
              if (field.Type == TType.Struct) {
                User = new HttpServiceData.UserData.UserRep();
                User.Read(iprot);
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.I32) {
                LeaderboardScore = iprot.ReadI32();
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
        TStruct struc = new TStruct("LeaderboardUserRep");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (User != null && __isset.user) {
          field.Name = "user";
          field.Type = TType.Struct;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          User.Write(oprot);
          oprot.WriteFieldEnd();
        }
        if (__isset.leaderboardScore) {
          field.Name = "leaderboardScore";
          field.Type = TType.I32;
          field.ID = 2;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(LeaderboardScore);
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
      StringBuilder __sb = new StringBuilder("LeaderboardUserRep(");
      bool __first = true;
      if (User != null && __isset.user) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("User: ");
        __sb.Append(User== null ? "<null>" : User.ToString());
      }
      if (__isset.leaderboardScore) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("LeaderboardScore: ");
        __sb.Append(LeaderboardScore);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
