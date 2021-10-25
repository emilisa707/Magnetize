using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2D;

    public float moveSpeed = 5f;

    public float pullForce = 100f;
    public float rotateSpeed = 360f;

    private GameObject closestTower;
    private GameObject hookedTowerToLeft;
    private GameObject hookedTowerToRight;

    private bool isPulled = false;

    private UIControllerScript uiControl;

    private AudioSource myAudio;
    private bool isCrashed = false;

    Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        uiControl = GameObject.Find("Canvas").GetComponent<UIControllerScript>();

        myAudio = this.gameObject.GetComponent<AudioSource>();

        startPosition = this.transform.position;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPulled)
        {
            /*if (closestTower != null && hookedTowerToRight == null)
            {
                
            }

            if (closestTower != null && hookedTowerToLeft == null)
            {
                
            }*/

            if (hookedTowerToRight)
            {
                float distance = Vector2.Distance(transform.position, hookedTowerToRight.transform.position);

                Vector3 pullDirection = (hookedTowerToRight.transform.position - transform.position).normalized;

                float newPullForce = Mathf.Clamp(pullForce / distance, 20, 50);
                rb2D.AddForce(pullDirection * newPullForce);

                rb2D.angularVelocity = -rotateSpeed / distance;
                isPulled = true;
            }
            
            if (hookedTowerToLeft)
            {
                float distance = Vector2.Distance(transform.position, hookedTowerToLeft.transform.position);

                Vector3 pullDirection = (hookedTowerToLeft.transform.position - transform.position).normalized;

                float newPullForce = Mathf.Clamp(pullForce / distance, 20, 50);
                rb2D.AddForce(pullDirection * newPullForce);

                rb2D.angularVelocity = rotateSpeed / distance;
                isPulled = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPulled = false;
            rb2D.angularVelocity = 0;
        }


        if (isCrashed)
        {
            if (!myAudio.isPlaying)
            {
                RestartPosition();
            }
        }


        else {
            rb2D.velocity = -transform.up * moveSpeed;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            //uiControl.RestartGame();

            if(!isCrashed)
            {
                myAudio.Play();
                rb2D.velocity = new Vector3(0f, 0f, 0f);
                rb2D.angularVelocity = 0f;
                isCrashed = true;
            } 
        }
    }
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Debug.Log("Levelclear!");
            uiControl.EndGame();
        }
    }
    
    public void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "TowerToLeft")
        {
            closestTower = collision.gameObject;
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            hookedTowerToLeft = closestTower;
            hookedTowerToRight = null;
        }
        else if(collision.gameObject.tag == "TowerToRight")
        {
            closestTower = collision.gameObject;
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            hookedTowerToRight = closestTower;
            hookedTowerToLeft = null;
        }
    }
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (isPulled) return;

        if(collision.gameObject.tag == "TowerToRight" || collision.gameObject.tag == "TowerToRight")
        {
            closestTower = null;

            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    
    public void RestartPosition()
    {
        uiControl.experimentTo += 1;
        this.transform.position = startPosition;

        this.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        isCrashed = false;

        if(closestTower)
        {
            closestTower.GetComponent<SpriteRenderer>().color = Color.white;
            closestTower = null;
        }
    }
}
