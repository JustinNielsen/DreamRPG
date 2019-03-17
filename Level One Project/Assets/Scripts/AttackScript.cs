using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    //hitbox is the child of the player
    private GameObject player;
    public GameObject hitbox;
    public Collider hitboxCollider;
    //prefab
    public GameObject[] prefab;
    //Collider
    PlayerController pController;
    public GameObject mageShot;
    public LineRenderer line;
    public HUD hud;
    public int spellCost;
    public int damage;
    public int savedSpellCost;
    public int savedDamage;

    //Start Function
    private void Start()
    {
        //Initialize the boi game object
        player = this.gameObject;
        //Initilaize player controller
        pController = GetComponent<PlayerController>();
        //Initilize LineRendrer
        line = player.GetComponent<LineRenderer>();
        //Initilize spell cost to 30
        spellCost = 20;
        //Initilize player damage to 1
        damage = 2;
    }

    private void Update()
    {

        if(pController.state == States.RangeAttack && pController.canAttack && pController.active)
        {
            //Shoot the projectile in the players forward direction
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Shoot");

                //Only cast spell if the player has enough mana
                if (pController.remainingMana >= spellCost)
                {
                    StartCoroutine(LaunchProjectile());
                    hud.DecreaseManaBar(spellCost);
                    pController.movement.anim.SetTrigger("Shoot");
                }
                else
                {
                    Debug.Log("Not enough Mana");

                    //Notify player when they don't have enoguh mana to cast a spell
                    StartCoroutine(hud.DisplayError("Not Enough Mana"));
                }
            }
        }

        if(pController.state == States.MeleeAttack && pController.canAttack && pController.active)
        {
            //Enables the collider
            if (Input.GetMouseButtonDown(0))
            {
                //Note: The following code is only slightly useful if we want to make it so you only melee attack once per turn.
                Debug.Log(pController.meleeAttacked);
                //If the player hasn't melee attacked yet attack otherwise don't attack
                if (!pController.meleeAttacked)
                {
                    pController.MattVoiceOver(Random.Range(15, 16));
                    StartCoroutine(Hit());
                    pController.movement.anim.SetTrigger("Attack");
                }
                else
                {
                    Debug.Log("Already Melee Attacked");

                    //Notify person playing game that they can't attack again
                    StartCoroutine(hud.DisplayError("Out of Melee Attacks"));
                }
            }
        }
    }

    //The main attacking function
    public void MeleeAttackMode()
    {
        //Choose the attack. Runs once per R press /// TODOODODODODODOD
        if (hitbox == null)
        {
            //Basic attack... Kind of.
            CallCollider(-2, 0.5f, 0, prefab[0]);

        }

        //Turns the player towards the mouse pointer
        LookAtMouse();

        
    }

    public void RangeAttackMode()
    {
        //Set the lines color to blue
        line.material.color = Color.blue;

        //Looks at the mosue pointer
        LookAtMouse();

        Vector3 lineStart = transform.position;
        lineStart.y = transform.position.y + (player.transform.localScale.y * 1.3f);

        //Sets the line to two points and sets the first point to the players location
        line.positionCount = 2;
        line.SetPosition(0, lineStart + transform.forward * pController.gameObject.transform.localScale.y);

        RaycastHit hit;

        //Draws the line 20 units forward unless obstructed by an object
        if (Physics.Raycast(lineStart, transform.forward, out hit, 20f))
        {
            line.SetPosition(1, hit.point);
        }
        else
        {
            line.SetPosition(1, transform.forward * 20 + lineStart);
        }


    }

    //Points the player towards the mouse
    public void LookAtMouse()
    {
        Ray cameraRay = pController.cam.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 11;

        RaycastHit rayHit;

        if (Physics.Raycast(cameraRay, out rayHit, Mathf.Infinity, layerMask))
        {
            if (rayHit.collider.tag == "ground")
            {
                Vector3 pointToLook = new Vector3(rayHit.point.x, transform.position.y, rayHit.point.z);
                Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);
                player.transform.LookAt(pointToLook);
            }
        }
    }

    //TODO - Instead of instatiating the hitbox have it as a child of the player and enable/disable the mesh renderer when in melee attack state
    private void CallCollider(float offsetX, float offsetY, float offsetZ, GameObject prefab)
    {
        /* 
         * Level 1: Forward: 1 Y: 1.25 Scale: 1.75, 0.5, 1
         * Level 2: Forward: 0.75 Y: 1.4 Scale: 1.75, 0.5, 1
         * Level 3: Forward: Y: Scale: 
         * Level 4: Forward: 0.75 Y:1.3 Scale: 
        */


        //Creates a vector at the player's current position.
        //Vector3 pos = player.transform.position + (transform.forward * 0.75f);
        Vector3 pos = player.transform.position;
        //Vector3 pos = new Vector3(0, 1.3f, 0.75f);
        //pos.y += 1.3f;
        //Creates Collider at player's position, with the player as a parent.
        hitbox = Instantiate(prefab, pos, player.transform.rotation, player.transform);
        //Disables the collider
        hitboxCollider = hitbox.GetComponent<Collider>();
        //Sets the damage of the melee attack
        hitbox.GetComponent<Damage>().damage = damage;
        //Set the local position of the object
        hitbox.transform.localPosition = new Vector3(0, 1.3f, 0.75f);
    }

    //Creates a projectile object from the mageshot prefab and destoys it after 1.6 seconds
    IEnumerator LaunchProjectile()
    {
        //Vector3 pos = transform.position + transform.forward;

        pController.canAttack = false;

        //Wait for the animation to catch up
        yield return new WaitForSeconds(0.8f);

        pController.canAttack = true;

        if (pController.lController.levels == Levels.Space)
        {
            Vector3 lineStart = transform.position + (transform.forward * pController.gameObject.transform.localScale.y);
            lineStart.y = transform.position.y + (player.transform.localScale.y * 1.3f);

            GameObject projectile = Instantiate(mageShot, lineStart, transform.rotation);
            Destroy(projectile, 1.6f);
        }
        else
        {
            Vector3 lineStart = transform.position + (transform.forward * 2f);
            lineStart.y = transform.position.y + (player.transform.localScale.y * 1.3f);

            GameObject projectile = Instantiate(mageShot, lineStart, transform.rotation);
            Destroy(projectile, 1.6f);
        }

        //Play Magic Sound
        pController.PlayPlayerSounds(0);
    }

    IEnumerator Hit()
    {
        pController.canAttack = false;

        yield return new WaitForSeconds(0.7f);

        hitboxCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        hitboxCollider.enabled = false;

        pController.canAttack = true;

        //Bool stops the player from melee attacking again
        pController.meleeAttacked = true;
    }


}
