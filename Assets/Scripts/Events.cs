using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class is where we will put specific Events that we can call and listen for
public class Events
{
    #region Tutorial
    
    //When you make a new Event, it has to be a class that inherits from GameEvent
    public class ExampleEvent : GameEvent
    {
        //Some Events you can keep empty, but sometimes you will want to pass variables around with an event
        //In that case you will create a public Getter variable of whatever type you need
        public int GetVar { get; }

        //In both empty and full Events, you will need to create a public function with the same name as the Event
        //This is what you will call when calling the Event
        public ExampleEvent(int getVar)
        {
            //If the Event has a variable that needs to be passed, you will pass it through the function like this
            GetVar = getVar;
        }
    }

    //In order to receive your Events, you will need to set up Handlers in each script you want to receive an Event from
    
    //In the Start() function of the script, you will type EventManager.Instance.AddHandler<ExampleEvent>(OnExampleEvent)
    
    //In the OnDestroy() function of the script, you MUST add EventManager.Instance.RemoveHandler<ExampleEvent>(OnExampleEvent)
    
    //YOU MUST REMOVE ALL HANDLERS IN ONDESTROY OR YOU WILL CAUSE MEMORY LEAKS
    
    //Once you have your Handler, you must create your OnExampleEvent function. Create it as public as you would any function.
    //This function must include a constructor that handles the event.
    //public void OnExampleEvent(ExampleEvent evt){}
    //Now you can fill in that function with whatever you want it to do
    //If you want to use any of the variables in the Event, use evt.GetVar (evt."variableName")
    
    //To Fire an Event, all you have to do is call EventManager.Instance.Fire(new ExampleEvent(VariableValue));
    //When this happens, all scripts that have a Handler for this Event will be informed of the Event
    
    #endregion
}
