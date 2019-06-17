// ------------------------------------------------------------------
// Description : 状态机
// Author      : leonliu
// Date        : 2015.04.09
// Histories   :
// ------------------------------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using QGame.Core.FightEnegin;

namespace QGame.Core.StateMachine
{
    /// <summary>
    /// 状态机默认状态为第一个状态。。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StateMachine<T>
    {
        T currentState;//初始化时会自动取第一个状态
        Action currentAction;
        /// <summary>
        /// 状态切换事件
        /// </summary>
        Action currentChangeAction;

        public Dictionary<T, Action> StateFunctions;
        public Dictionary<T, Action> StateChangeFunctions;

        public StateMachine()
        {
            StateFunctions = new Dictionary<T, Action>();
            StateChangeFunctions = new Dictionary<T, Action>();
        }

        public T CurrentState { get { return currentState; } }

        public bool PerformAction()
        {
            return PerformAction(currentState);
        }

        private bool PerformAction(T action)
        {
            if (null != currentChangeAction)
            {              
                currentChangeAction();
                int a = (int)Convert.ChangeType(action, typeof(int));
                int c = (int)Convert.ChangeType(currentState, typeof(int));

                if (a == c)//只处理当前状态的切换事件
                {
                    currentChangeAction = null;
                }
                else
                {
                    //在currentChangeAction事件中切换了状态 currentAction不执行了，因为currentAction也会被改变
                   // Debug.LogError("状态变了！！！！！！！！！！！！！！！！   ");
                    return true;
                }

            }
            if (null != currentAction)
                currentAction();
            return true;
        }

        public bool CompareState(T state)
        {
            int s = (int)Convert.ChangeType(state, typeof(int));
            int c = (int)Convert.ChangeType(currentState, typeof(int));          
            return s == c;
        }

        virtual public bool SetState(T to)
        {
            if (CompareState(to))
            {
                return false;
            }
            else
            {
                if (StateFunctions.ContainsKey(to))
                {
                    currentState = to;
                    currentAction = StateFunctions[currentState];
                   
                    currentChangeAction = StateChangeFunctions[currentState];//执行首次进入状态事件
                  
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 增加状态
        /// </summary>
        /// <param name="statename">状态</param>
        /// <param name="function">帧执行</param>
        /// <param name="changeFunction">首次进入事件 执行优先于帧执行function</param>
        public void AddState(T statename, Action function = null,Action changeFunction=null)
        {              
            if (null == statename)
            {
                return;
            }

            if (StateFunctions.ContainsKey(statename))
            {
                StateFunctions[statename] = function;
                StateChangeFunctions[statename] = changeFunction;
            }
            else
            {
                StateFunctions.Add(statename, function);
                StateChangeFunctions.Add(statename, changeFunction);
                if (StateFunctions.Count == 1)
                {
                    if (StateFunctions.ContainsKey(currentState))
                        currentAction = StateFunctions[currentState];
                    if (StateChangeFunctions.ContainsKey(currentState))
                        currentChangeAction = StateChangeFunctions[currentState];
                }
               
            }
         
        }

        public bool RemoveState(T statename)
        {
            if (StateChangeFunctions.ContainsKey(statename))
            {
                StateChangeFunctions.Remove(statename);
            }
            if (StateFunctions.ContainsKey(statename))
            {
                StateFunctions.Remove(statename);
                return true;
            }
            return false;
        }
    }
}
