using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;

public class TestUnityJobs : MonoBehaviour{
    [SerializeField] private bool useJobs = false;

    private void Update(){
        float startTime = Time.realtimeSinceStartup;
        if (!useJobs){
            for(int i= 0; i < 10; i++){
                HeavyDuty();
            }

        }
        else
        {
            JobHandle[] jobHandles = new JobHandle[10];
            for(int i=0; i< 10; i++){
                jobHandles[i] = HeavyDutyJob();
            }
            NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(jobHandles, Allocator.Temp);
            JobHandle.CompleteAll(jobHandleArray);
            jobHandleArray.Dispose();
        }

        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000f) + "(ms)");
    }

    private void HeavyDuty(){
        double value = 1;
        for(int i = 0; i < 50000; i++)
        {
            value = (double)Math.Exp((double)Math.Sqrt(value));
        }
        Debug.Log("Final value: " + value);
    }

    private JobHandle HeavyDutyJob()
    {
        ReallyHeavyTask reallyHeavyTask = new ReallyHeavyTask();
        return reallyHeavyTask.Schedule();
    }
}

[BurstCompile]
public struct ReallyHeavyTask : IJob
{
    public void Execute(){
        double value = 1;
        for (int i = 0; i < 50000; i++)
        {
            value = (double)Math.Exp((double)Math.Sqrt(value));
        }
    }
}
