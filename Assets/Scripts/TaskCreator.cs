//  Created by Javier Alejandro Domínguez Falcón on 22/05/2020.
//  Copyright © 2020 Javier Alejandro Domínguez Falcón. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskCreator : MonoBehaviour
{
    private TaskRunner _taskRunner;
    public Text messageText;

    private void Start()
    {
        _taskRunner = GetComponent<TaskRunner>();

        _taskRunner.Enqueue("task1", RunningLongLastingTasks, OnTaskCompletedInMainThread);
        _taskRunner.Enqueue("task2", RunningLongLastingTasks, OnTaskCompletedInMainThread);
        _taskRunner.Enqueue("task3", RunningLongLastingTasks, OnTaskCompletedInMainThread);
    }

    public void RunningLongLastingTasks()
    {
        //This action will be executed in a separate thread
        //You can't work with Unity Functions here because we are in different Unity main thread

        // This code won't work 
        //messageText = GetComponentInChildren<Text>();
        //messageText.text = "Enter";
    }

    public void OnTaskCompletedInMainThread()
    {
        //This action will be executed in Unity Main Thread
        //We can work with Unity Functions here because we are in the same Unity main thread

        // This code will work on Unity Main Thread 
        messageText = GetComponentInChildren<Text>();
        messageText.text = "Enter";
    }
}
