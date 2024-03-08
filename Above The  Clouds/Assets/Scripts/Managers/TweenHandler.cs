using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TweenHandler : Singleton<TweenHandler>{
    private Dictionary<GameObject, Tuple<Task, CancellationTokenSource>> _tasks = new Dictionary<GameObject, Tuple<Task, CancellationTokenSource>>();

    private void OnDisable(){
        while(_tasks.Count > 0){
            foreach (var task in _tasks.Keys){
                _tasks[task].Item2?.Cancel();
                _tasks?.Remove(task);
                break;
            }
        }
    }

    public void CreateTween(TweenParameters tweenParameters){
        GameObject tweenable = tweenParameters.GetTweenable();
        RemoveKey(tweenable);

        CancellationTokenSource c = new CancellationTokenSource();
        Task tweenTask = Tween(tweenParameters, c);
        _tasks[tweenable] = Tuple.Create(tweenTask, c);
    }

    private async Task Tween(TweenParameters tweenParameters, CancellationTokenSource c){
        GameObject tweenable = tweenParameters.GetTweenable();
        float timeout = tweenParameters.GetTimeOut();
        while (true){
            tweenParameters.TweenAll();
            timeout -= Time.deltaTime;
            if (timeout <= 0){
                tweenParameters.ExecuteCallback();
                break;
            }
            await Task.Delay((int)(Time.deltaTime * 1000), c.Token);
        }
        CancelTween(tweenable);

    }

    public void CancelTween(GameObject source) => RemoveKey(source);

    private void RemoveKey(GameObject source){
        if (_tasks.ContainsKey(source)){
            _tasks[source].Item2?.Cancel();
            _tasks.Remove(source);
        }
    }
}