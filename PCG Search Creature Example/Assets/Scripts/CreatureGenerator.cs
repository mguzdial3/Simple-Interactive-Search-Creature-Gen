using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureGenerator : MonoBehaviour
{
    public LimbObject limbPrefab;

    private float minScale = 0.2f, maxScale = 3;
    private float minForce = 1, maxForce = 8;
    private float minIncremement = 0.5f, maxIncrement= 2f;
    private float minRotation = -30f, maxRotation= 30f;

    private int minLimbs = 2, maxLimbs = 6;

    public LimbObject currCreatureObj;
    public Limb currCreature, currBest;

    //
    private float maxLife = 5, origMaxLife;
    private float currLife = 0;

    private float fitness = 0;

    public bool hillClimbing = false;

    // Start is called before the first frame update
    void Start()
    {
        RandomCreature();
        InstantiateCreature();
        origMaxLife = maxLife;
    }

    private void RandomCreature(){
        Limb head = GenerateCreature();
        currCreature = head;
    }

    private void InstantiateCreature(){
        List<Limb> openSet = new List<Limb>();
        openSet.Add(currCreature);

        while(openSet.Count>0){
            Limb currLimb = openSet[0];
            openSet.Remove(currLimb);
            LimbObject limbObj = Instantiate(limbPrefab, new Vector3(0,0,0), Quaternion.identity);
            limbObj.myLimb = currLimb;
            currLimb.obj = limbObj;

            if(currLimb.parent!=null)
            {
                GameObject fulcrum = new GameObject("fulcrum");
                fulcrum.transform.position = currLimb.parent.obj.gameObject.transform.position;
                fulcrum.transform.parent = currLimb.parent.obj.gameObject.transform;
                limbObj.gameObject.transform.parent = fulcrum.transform;
                limbObj.gameObject.transform.position = currLimb.offset;
            }

            limbObj.gameObject.transform.localScale = currLimb.scale;

            foreach (Limb child in currLimb.children){
                openSet.Add(child);
            }
        }

        currCreatureObj= currCreature.obj;
    }

    void Update()
    {
        currLife+=Time.deltaTime;
        if(currLife >=maxLife){
            float thisFitness = currCreatureObj.gameObject.transform.position.magnitude;
            if(thisFitness>fitness){
                fitness = thisFitness;
                currBest = currCreature;

            }

            Destroy(currCreatureObj.gameObject);
            if(!hillClimbing){
                RandomCreature();
            }
            else{
                currCreature = CloneAndRandomlyAlterCreature(currBest);
            }
            InstantiateCreature();
            currLife=0;
            maxLife = origMaxLife;
        }

        if(Input.GetKeyDown(KeyCode.B)){
            Destroy(currCreatureObj.gameObject);
            currCreature = currBest;
            currLife= 0;

            InstantiateCreature();
            maxLife = origMaxLife*2;
        }

        if(Input.GetKeyDown(KeyCode.H)){
            hillClimbing=true;   
        }
        if(Input.GetKeyDown(KeyCode.R)){
            hillClimbing=false;   
        }
    }

    private Limb RandomLimb (Limb _parent){
        float xScale = Random.Range(minScale, maxScale);
        float yScale = Random.Range(minScale, maxScale);
        float zScale = Random.Range(minScale, maxScale);

        float xOffset = 0;
        float yOffset = 0;
        float zOffset = 0;

        if(_parent!=null){
            xOffset = Random.Range(-1*_parent.scale.x, _parent.scale.x);
            yOffset = Random.Range(-1*_parent.scale.y, _parent.scale.y);
            zOffset = Random.Range(-1*_parent.scale.z, _parent.scale.z);
        }

        Vector3 force = new Vector3(Random.Range(minForce, maxForce),Random.Range(minForce, maxForce),Random.Range(minForce, maxForce));
        Vector3 rotation = new Vector3(Random.Range(minRotation, maxRotation),Random.Range(minRotation, maxRotation),Random.Range(minRotation, maxRotation));

        float increment = Random.Range(minIncremement, maxIncrement);

        return new Limb(new Vector3(xScale, yScale, zScale), new Vector3(xOffset, yOffset, zOffset), force, rotation, increment, _parent);
    }


    private Limb GenerateCreature(){
        Limb head = RandomLimb(null);

        int generatedLimbs = 1;
        Limb currLimb = head;
        int numLimbs = Random.Range(minLimbs, maxLimbs);

        while(generatedLimbs<numLimbs){
            Limb newLimb = RandomLimb(currLimb);
            currLimb.children.Add(newLimb);

            if (Random.Range(0f,1f)<0.5f){
                currLimb = newLimb;
            }

            generatedLimbs+=1;
        } 

        return head;
    }

    private Limb CloneCreature(Limb orig){
        List<Limb> openSet = new List<Limb>();
        openSet.Add(orig);

        
        List<Limb> oldLimbs = new List<Limb>();
        List<Limb> newLimbs = new List<Limb>();

        int iterations = 0;
        while(openSet.Count>0){
            Limb currLimb = openSet[0];
            openSet.Remove(currLimb);

            Limb newLimb = new Limb(currLimb.scale, currLimb.offset, currLimb.force, currLimb.rotation, currLimb.increment, null);

            oldLimbs.Add(currLimb);
            newLimbs.Add(newLimb);

            foreach (Limb child in currLimb.children){
                openSet.Add(child);
            }
        }

        for(int i = 0; i< oldLimbs.Count; i++){
            foreach(Limb child in oldLimbs[i].children){
                newLimbs[oldLimbs.IndexOf(child)].parent = newLimbs[i];
                newLimbs[i].children.Add(newLimbs[oldLimbs.IndexOf(child)]);
            }
        }

        return newLimbs[0];
    }

    private Limb CloneAndRandomlyAlterCreature(Limb orig){
        List<Limb> openSet = new List<Limb>();
        openSet.Add(orig);

        
        List<Limb> oldLimbs = new List<Limb>();
        List<Limb> newLimbs = new List<Limb>();

        int iterations = 0;
        while(openSet.Count>0){
            Limb currLimb = openSet[0];
            openSet.Remove(currLimb);

            Limb newLimb = new Limb(currLimb.scale, currLimb.offset, currLimb.force, currLimb.rotation, currLimb.increment, null);

            oldLimbs.Add(currLimb);
            newLimbs.Add(newLimb);

            foreach (Limb child in currLimb.children){
                openSet.Add(child);
            }
        }

        for(int i = 0; i< oldLimbs.Count; i++){
            foreach(Limb child in oldLimbs[i].children){
                newLimbs[oldLimbs.IndexOf(child)].parent = newLimbs[i];
                newLimbs[i].children.Add(newLimbs[oldLimbs.IndexOf(child)]);
            }
        }

        //Randomly alter

        int indexToAlter = Random.Range(0, newLimbs.Count);

        int randomChange = Random.Range(0,6);

        if(randomChange==0){
            float xScale = Random.Range(minScale, maxScale);
            float yScale = Random.Range(minScale, maxScale);
            float zScale = Random.Range(minScale, maxScale);

            newLimbs[indexToAlter].scale = new Vector3(xScale, yScale, zScale);

        }
        else if (randomChange==1){
            if(newLimbs[indexToAlter].parent!=null){
                float xOffset = Random.Range(-1*newLimbs[indexToAlter].parent.scale.x, newLimbs[indexToAlter].parent.scale.x);
                float yOffset = Random.Range(-1*newLimbs[indexToAlter].parent.scale.y, newLimbs[indexToAlter].parent.scale.y);
                float zOffset = Random.Range(-1*newLimbs[indexToAlter].parent.scale.z, newLimbs[indexToAlter].parent.scale.z);

                newLimbs[indexToAlter].offset = new Vector3(xOffset, yOffset, zOffset);
            }
        }
        else if (randomChange==2){
            newLimbs[indexToAlter].force = new Vector3(Random.Range(minForce, maxForce),Random.Range(minForce, maxForce),Random.Range(minForce, maxForce));
        }
        else if(randomChange==3){
            newLimbs[indexToAlter].rotation = new Vector3(Random.Range(minRotation, maxRotation),Random.Range(minRotation, maxRotation),Random.Range(minRotation, maxRotation));
        }
        else{
            newLimbs[indexToAlter].increment = Random.Range(minIncremement, maxIncrement);
        }

        return newLimbs[0];
    }
}
