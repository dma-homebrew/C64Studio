﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;



namespace C64Studio.Tasks
{
  public class TaskManager
  {
    private List<Tasks.Task>      m_TaskQueue = new List<Tasks.Task>();

    private BackgroundWorker      m_BackgroundWorker = new BackgroundWorker();

    private StudioCore            Core;



    public TaskManager( StudioCore Core )
    {
      this.Core = Core;
      m_BackgroundWorker.DoWork += m_BackgroundWorker_DoWork;
    }



    void m_BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      while ( !Core.ShuttingDown  )
      {
        Tasks.Task    newTask = null;
        lock ( m_TaskQueue )
        {
          if ( m_TaskQueue.Count == 0 )
          {
            return;
          }
          newTask = m_TaskQueue[0];
          m_TaskQueue.RemoveAt( 0 );
        }
        newTask.RunTask();
      }
    }



    public void AddTask( Tasks.Task Task )
    {
      Task.Core = Core;
      lock ( m_TaskQueue )
      {
        var tasksToRemove = new List<Task>();

        /*
        if ( Task is TaskUpdateKeywords )
        {
          Debug.Log( "Add TaskUpdateKeywords for " + ( (TaskUpdateKeywords)Task ).Doc.DocumentFilename );
        }*/

        // remove duplicate update keyword tasks!
        foreach ( var task in m_TaskQueue )
        {
          if ( ( task is TaskUpdateKeywords )
          &&   ( Task is TaskUpdateKeywords ) )
          {
            var  oldTask  = task as TaskUpdateKeywords;
            var  newTask  = Task as TaskUpdateKeywords;

            if ( oldTask.Doc == newTask.Doc )
            {
              //Debug.Log( "remove previous TaskUpdateKeywords as duplicate" );
              tasksToRemove.Add( oldTask );
            }
          }
        }
        foreach ( var taskToRemove in tasksToRemove )
        {
          m_TaskQueue.Remove( taskToRemove );
        }

        m_TaskQueue.Add( Task );
      }
      if ( !m_BackgroundWorker.IsBusy )
      {
        m_BackgroundWorker.RunWorkerAsync();
      }
    }

  }
}
