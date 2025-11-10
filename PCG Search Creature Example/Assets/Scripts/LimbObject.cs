using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbObject : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public Limb myLimb;
    private float timer = 0;
    private bool oneWayOrOther;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;

        if(transform.parent!=null){
            myRigidbody.isKinematic = true;   
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;

        if(timer>=myLimb.increment){
            timer=0;
            myRigidbody.AddForce(myLimb.force);
            oneWayOrOther=!oneWayOrOther;
        }
        else{
            if(transform.parent!=null){
                if(oneWayOrOther){
                    transform.parent.Rotate(Time.deltaTime*myLimb.rotation);
                }
                else{
                    transform.parent.Rotate(-1*Time.deltaTime*myLimb.rotation);
                }
            }
            else{
                if(oneWayOrOther){
                    transform.Rotate(Time.deltaTime*myLimb.rotation);
                }
                else{
                    transform.Rotate(-1*Time.deltaTime*myLimb.rotation);
                }
            }
        }
    }
}
