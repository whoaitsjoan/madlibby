using Naninovel.Spreadsheet;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;
    private Rigidbody2D myRB;
    private Vector3 change;

    void Start()
    {
        myRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        Movement();
    }

    void Movement()
    {

        if (Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f)
        {
            myRB.transform.Translate(new Vector3(change.x * speed * Time.deltaTime, 0f, 0f));

        }
        else if (Input.GetAxisRaw("Vertical") > 0.5f || Input.GetAxisRaw("Vertical") < -0.5f)
        {
            myRB.transform.Translate(new Vector3(0, change.y * speed * Time.deltaTime, 0f));


        }



    }
}
