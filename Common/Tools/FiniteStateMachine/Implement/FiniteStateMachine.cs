using System.Collections.Generic;
using UnityEngine;

namespace com.snake.framework
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FiniteStateMachine<T> where T : class, IFiniteStateMachineOwner
    {
        /// <summary>
        /// 状态机持有者
        /// </summary>
        public T mOwner { get; protected set; }

        /// <summary>
        /// 状态字典缓存
        /// </summary>
        private Dictionary<string, IState<T>> _stateDic;

        /// <summary>
        /// 当前状态
        /// </summary>
        private IState<T> _currState;

        /// <summary>
        /// 状态切换中
        /// </summary>
        private bool _switching;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="owner"></param>
        public FiniteStateMachine(T owner)
        {
            this.mOwner = owner;
            this._stateDic = new Dictionary<string, IState<T>>();
            this._switching = false;
        }

        /// <summary>
        /// 是否允许切换
        /// </summary>
        /// <returns></returns>
        public bool CanSwitch()
        {
            return this._switching == false;
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public bool Switch(string stateName, object userData)
        {
            if (HasState(stateName) == false || _switching == true)
                return false;

            _switching = true;
            IState<T> toState = GetState(stateName);
            if (_currState != null)
                _currState.Exit(mOwner, toState);
            toState.Enter(mOwner, _currState, userData);
            _currState = toState;
            _switching = false;
            return true;
        }

        /// <summary>
        /// 生命周期 - 每帧更新
        /// </summary>
        /// <param name="frameCount"></param>
        /// <param name="time"></param>
        /// <param name="deltaTime"></param>
        /// <param name="unscaledTime"></param>
        /// <param name="realElapseSeconds"></param>
        public void Tick(int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
        {
            _currState?.Tick(mOwner, frameCount, time, deltaTime, unscaledTime, realElapseSeconds);
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public IState<T> GetState(string stateName)
        {
            IState<T> state;
            if (this._stateDic.TryGetValue(stateName, out state))
                return state;
            return null;
        }

        /// <summary>
        /// 增加状态
        /// </summary>
        /// <typeparam name="IS"></typeparam>
        /// <returns></returns>
        public void AddState(string stateName, IState<T> stateTarget, bool replace)
        {
            if (HasState(stateName) == true && replace == false)
            {
                Debug.LogErrorFormat("状态已存在，添加状态失败. State:{0}", stateName);
                return;
            }
            this._stateDic.Add(stateName, stateTarget);
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public bool RemoveState(string stateName)
        {
            IState<T> state = GetState(stateName);
            if (state == null)
                return false;
            state.Dispose(this.mOwner);
            return true;
        }

        /// <summary>
        /// 是否存在状态
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public bool HasState(string stateName)
        {
            IState<T> state;
            return this._stateDic.TryGetValue(stateName, out state);
        }
    }
}
