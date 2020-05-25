//  Created by Javier Alejandro Domínguez Falcón on 22/05/2020.
//  Copyright © 2020 Javier Alejandro Domínguez Falcón. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Write a Producer/Consumer like task runner in Unity to be able to run long lasting tasks on different threads. Each task should have a complete callback that gets called on Unitys main thread. Adding new tasks can be restricted from the Unity main thread only for sake of simplicity.
/// </summary>
public class TaskRunner : MonoBehaviour
{
    private static readonly Queue<TaskLasting> _executionQueue = new Queue<TaskLasting>();

    private void Start()
    {
            
    }

    // Unity life-cycle update method, controls the queues
    public void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                var currentTaskLasting = _executionQueue.Dequeue();
                RunAsync(currentTaskLasting.message, currentTaskLasting.LongLastingTask, currentTaskLasting.TaskOnMainThread);
            }
        }
    }

    /// <summary>
    /// Locks the queue and adds the Task to the queue
    /// </summary>
    /// <param name="action">Action that will be executed in another thread if necessary.</param>
    /// <param name="action">Action that will be executed from the main thread.</param>
    public void Enqueue(string message, Action action, Action onActionCompleted)
    {
        TaskLasting taskLasting = new TaskLasting();
        taskLasting.CreateTaskLasting(message, action, onActionCompleted);

        lock (_executionQueue)
        {
            //_actionPairs.Add(action, onActionCompleted);
            _executionQueue.Enqueue(taskLasting);
        }
    }

    // API methods to run an action on the main thread
    public static void QueueOnMainThread(Action action)
    {
        action.Invoke();
        ShowThreadInfo("Main Thread ");
    }

    // API method to run an action on a thread
    public async static Task RunAsync(string m, Action a, Action onActionCompleted)
    {
        // Execute in other thread this Task and after finish return to main thread
        await Task.Run(() => {
            a.Invoke();
            ShowThreadInfo("RunAsync " + m);
        });

        //Returned to the main thread
        QueueOnMainThread(onActionCompleted);
    }

    static void ShowThreadInfo(string s)
    {
        Console.WriteLine("{0} Thread ID: {1}", s, Thread.CurrentThread.ManagedThreadId);
        Debug.LogFormat("{0} Thread ID: {1}", s, Thread.CurrentThread.ManagedThreadId);
    }
}

public struct TaskLasting
{
    public string message;
    public Action LongLastingTask;
    public Action TaskOnMainThread;

    /// <summary>
    /// Create long lasting tasks
    /// </summary>
    /// <param name="longLastingTask"> Long lasting task that it'll be executed in diferents threads</param>
    /// <param name="onLongTaskCompleted">complete callback that gets called on Unitys main thread</param>
    public void CreateTaskLasting(string m, Action longLastingTask, Action onLongTaskCompleted)
    {
        message = m;
        LongLastingTask = longLastingTask;
        TaskOnMainThread = onLongTaskCompleted;
    }
}
