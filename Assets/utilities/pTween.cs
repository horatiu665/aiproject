using UnityEngine;
using System;
using System.Collections;

public class pTween 
{
    // Peter'unit Tweening Library.
    // Written by Peter Bruun-Rasmussen (http://www.bipbipspil.dk).
    
    /*  Example:
     *  
     *    IEnumerator Sequence()
     *    {
     *        Vector3 p1 = new Vector3(0,0,0);
     *        Vector3 p2 = new Vector3(0,10,0);
     *    
     *        yield return StartCoroutine(pTween.To(2f, t => { transform.position = Vector3.Lerp(p1, p2, t); }));
     *    }
     */

    public static IEnumerator To(float duration, float startValue, float endValue, Action<float> callback)
    {
        float start = Time.time;
        float end = start + duration;
        float durationInv = 1f / duration;
        float startMulDurationInv = start / duration;
        
        for(float t = Time.time; t < end; t = Time.time)
        {
            callback(Mathf.Lerp(startValue, endValue, t * durationInv - startMulDurationInv));
            yield return 0;
        }
        callback(endValue);
    }
	
    public static IEnumerator FixedTo(float duration, float startValue, float endValue, Action<float> callback)
    {
        float start = Time.time;
        float end = start + duration;
        float durationInv = 1f / duration;
        float startMulDurationInv = start / duration;
        
        for(float t = Time.time; t < end; t += Time.fixedDeltaTime)
        {
			yield return new WaitForFixedUpdate();
            callback(Mathf.Lerp(startValue, endValue, t * durationInv - startMulDurationInv));
           
        }
		yield return new WaitForFixedUpdate();
        callback(endValue);
    }	
	
    public static IEnumerator RealtimeTo(float duration, float startValue, float endValue, Action<float> callback)
    {
        float start = Time.realtimeSinceStartup;
        float end = start + duration;
        float durationInv = 1f / duration;
        float startMulDurationInv = start / duration;
        
        for(float t = Time.realtimeSinceStartup; t < end; t = Time.realtimeSinceStartup)
        {
            callback(Mathf.Lerp(startValue, endValue, t * durationInv - startMulDurationInv));
            yield return 0;
        }
        callback(endValue);
    }
    
    public static IEnumerator To(float duration, Action<float> callback)
    {
        return To(duration, 0f, 1f, callback);
    }
}
