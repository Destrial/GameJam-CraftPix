using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destrial
{
    public interface IEvent { }

    public struct LieEvent : IEvent
    {
        public bool isLie;
        
    }
    
    public struct RatCaptured : IEvent { }




    public struct DrawDoor : IEvent
    {
        public int Hinder;
    }
    
    public struct DoorReveal : IEvent
    {
        public bool WasMutant;
    }
  
    public struct TalkEvent : IEvent
    {
       
    }

    public struct StopTalkEvent: IEvent {}

}
