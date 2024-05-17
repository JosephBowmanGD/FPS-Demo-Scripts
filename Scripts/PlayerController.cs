using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
//using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviour //PunCallbacks, Damageable Move in to position later
{
    [Header("Refrences")]
    [SerializeField] private Item[] items;
    //PhotonView PhotonView; // Marks The proton view
    [SerializeField] private Transform playerCam;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject[] throwObject;

    [Header("Throwing Parametres")]
    [SerializeField] private KeyCode keyToThrow = KeyCode.G;
    [SerializeField] private float throwForce;
    [SerializeField] private float throwUpWardForce;
    [SerializeField] private int totalThrows;
    [SerializeField] private float throwCooldown;

    public int chosenGrenade; //Testing purposes only

    [Header("Throwing Bools")]
    bool readyToThrow;

    [Header("Index")]
    int itemIndex;
    int previousIndex = -1; // set to -1 as there is no previous index

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    //PlayerManager playerManager;

    private void Awake() {
        //PV = GetComponent<PhotonView>(); // Finds The proton view in the game
        //playerManager = PhotonView.Find(int)Pv.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start() {
        // if(PV.isMine)
        //     EquipItem(0); // Checks the current proton view and if its the players calls the equipItem method using an item index of zero
        //EquipItem(4); // Temp Delete Later
        readyToThrow = true;
    }
    
    private void Update() {
        if(Input.GetKey(KeyCode.O))
        {
            chosenGrenade = 0;
        }
        if(Input.GetKey(KeyCode.P))
        {
            chosenGrenade = 1;
        }

        if(Input.GetKey(keyToThrow) && readyToThrow && totalThrows > 0)
        {
            Throw();
        }


        for (int i = 0; i < items.Length; i++)
        {
            if(Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if(itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
                EquipItem(itemIndex + 1);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if(itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
                EquipItem(itemIndex - 1);
        }

        if(Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }
    }

    void Throw(){
        readyToThrow = false;

        //calculate rotations 
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, transform.forward);
        throwObject[chosenGrenade].transform.rotation = rot * throwPoint.transform.rotation;

        //create object
        GameObject thrownObject = Instantiate(throwObject[chosenGrenade], throwPoint.position, rot);

        Rigidbody thrownBody = thrownObject.GetComponent<Rigidbody>();

        //calculate forward direction
        Vector3 forceDirection = playerCam.transform.forward;

        RaycastHit hit;

        if(Physics.Raycast(playerCam.position, playerCam.forward, out hit, 500f)){
            forceDirection = (hit.point - throwPoint.position).normalized;
        }

        //Add force
        Vector3 forceAddition = forceDirection * throwForce + transform.up * throwUpWardForce;

        thrownBody.AddForce(forceAddition, ForceMode.Impulse);

        totalThrows--;

        //Throw cooldown
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    void ResetThrow(){
        readyToThrow = true;
    }

    void EquipItem(int index){
        if (index == previousIndex)
            return;

        itemIndex = index;
        items[itemIndex].itemGameObject.SetActive(true);

        if(previousIndex != -1){
            items[previousIndex].itemGameObject.SetActive(false);
        }

        previousIndex = itemIndex;

        /*if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash)'
        }*/
    }

    /*public override OnPlayerPropertiesUpdate(PlayerController targetPlayer, Hashtable changesProps) 
    {
        if(!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }*/

    public void TakeDamage(float damage)
    {
        //PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    /*[PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if(!PV.IsMine)
            return;

        currentHealth -= damage;
        
        if(currentHealth <= 0)
        {
            Die();
        }
    }*/

    void Die()
    {
        //playerManager.Die();
    }

}
