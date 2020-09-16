using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;

public class CharacterController2D : MonoBehaviourPun
{
    private Rigidbody2D rb;
    [SerializeField] private int health;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private float moveInput;

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isWalled;
    [SerializeField] private bool isDamaged;

    public Transform groundCheck;
    public Transform wallCheck;

    public float checkRadius;

    public LayerMask whatisGround;
    public LayerMask whatisWall;

    [SerializeField] private int extraJump;

    private bool facingRight = true;

    public Animator animator;
    public Slider healthBar;

    [SerializeField] private Text CountDowntext;
    private float startingTimer=30;

    private float currentTimer;

    public Text scoreDisplay;
    public GameObject WonPanel;
    public GameObject LostPanel;
    public GameObject TiePanel;
    public bool gameIsOver = false;
    public Text Player1Name;
    public Text Player2Name;
    public Text Player1Score;
    public Text Player2Score;

    private void Awake()
    {
        if (!SceneManager.GetActiveScene().name.Equals("WaitingScene"))
        {
            currentTimer = startingTimer;

            Player1Name = GameObject.FindGameObjectWithTag("Player1Name").GetComponent<Text>();
            Player1Score = GameObject.FindGameObjectWithTag("Player1Score").GetComponent<Text>();
            Player2Name = GameObject.FindGameObjectWithTag("Player2Name").GetComponent<Text>();
            Player2Score = GameObject.FindGameObjectWithTag("Player2Score").GetComponent<Text>();
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!SceneManager.GetActiveScene().name.Equals("WaitingScene"))
        {
            health = 100;
            PhotonNetwork.LocalPlayer.SetScore(0);
            healthBar = GameObject.FindWithTag("HealthSlider").GetComponent<Slider>();
            CountDowntext = GameObject.FindWithTag("Countdown").GetComponent<Text>();
            scoreDisplay = GameObject.FindWithTag("Score").GetComponent<Text>();

            WonPanel = GameObject.Find("WonPanel");
            WonPanel.SetActive(false);
            LostPanel = GameObject.Find("LostPanel");
            LostPanel.SetActive(false);  
        }
    }


    void FixedUpdate()
    {
        currentTimer -= Time.deltaTime;

        if (!SceneManager.GetActiveScene().name.Equals("WaitingScene"))
        {
                if (currentTimer <= 0)
                {
                    PhotonView.Get(this).RPC("CompareScores", RpcTarget.AllBufferedViaServer);
                    currentTimer = 0;
                }

                CountDowntext.text = currentTimer.ToString("0");
        }



        if (PhotonNetwork.IsConnected && photonView.IsMine)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatisGround);
            isWalled = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatisWall);

            Move();
            Jump();
            Damage();
            Health();

            animator.SetBool("isWalled", isWalled);

            if (!SceneManager.GetActiveScene().name.Equals("WaitingScene"))
            {
                healthBar.value = health;
            }
        }
    }

    #region Character Controller
    void Move()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        animator.SetFloat("Run", Mathf.Abs(moveInput));

        if (facingRight == false && moveInput > 0)
        {
            FlipCharater();
        }
        else if (facingRight == true && moveInput < 0)
        {
            FlipCharater();
        }
    }
    private void Jump()
    {
        if (isGrounded || isWalled)
        {
            extraJump = 1;
        }

        if (Input.GetKeyDown(KeyCode.Space) && extraJump > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            animator.SetBool("isJumping", true);
            extraJump--;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && extraJump == 0 && isGrounded == true)
        {
            rb.velocity = Vector2.up * jumpForce;
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }
    private void Damage()
    {

        if (isDamaged)
        {
            animator.SetBool("isDamaged", true);
            isDamaged = false;
            animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetBool("isDamaged", false);
        }
    }

    void FlipCharater()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    void Health()
    {
        if (health <= 0)
        {
            transform.position = new Vector3(-1.6f, -0.6f, 0f);
            health = 100;
        }
    }
    #endregion

    #region Collisions with objects
    void OnCollisionEnter2D(Collision2D player)
    {
        if (player.gameObject.CompareTag("Trap"))
        {
            rb.AddForce(new Vector2(speed, 7), ForceMode2D.Impulse);

            animator.SetBool("isJumping", false);
            animator.SetBool("isDamaged", true);

            health -= 10;
            isDamaged = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Fruit fruit = collision.GetComponent<Fruit>();


        if (fruit && photonView.IsMine)
        {
            IncreasePlayerScore(fruit.PointsOnPickup);
        }
    }
    #endregion

    #region Points Manager
    public void IncreasePlayerScore(int scoreToAdd)
    {
        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            PhotonNetwork.LocalPlayer.SetScore(PhotonNetwork.LocalPlayer.GetScore() + scoreToAdd);
            scoreDisplay.text = PhotonNetwork.LocalPlayer.GetScore().ToString();
        }

    }
    [PunRPC]
    public void CompareScores()
    {
        Player[] players = PhotonNetwork.PlayerList;

        if (players[0].GetScore() > players[1].GetScore())
        {
            if (PhotonNetwork.IsMasterClient)
            {
                WonPanel.SetActive(true);
                Player1Name.text = players[0].NickName.ToString();
                Player1Score.text = players[0].GetScore().ToString();
                Player2Name.text = players[1].NickName.ToString();
                Player2Score.text = players[1].GetScore().ToString();
            }
            else 
            {
                LostPanel.SetActive(true);
                Player1Name.text = players[0].NickName.ToString();
                Player1Score.text = players[0].GetScore().ToString();
                Player2Name.text = players[1].NickName.ToString();
                Player2Score.text = players[1].GetScore().ToString();
            }
        }
        else if (players[1].GetScore() > players[0].GetScore())
        {
            if (PhotonNetwork.IsMasterClient)
            {
                LostPanel.SetActive(true);
                Player1Name.text = players[0].NickName.ToString();
                Player1Score.text = players[0].GetScore().ToString();
                Player2Name.text = players[1].NickName.ToString();
                Player2Score.text = players[1].GetScore().ToString();
            }
            else 
            {
                WonPanel.SetActive(true);
                Player1Name.text = players[0].NickName.ToString();
                Player1Score.text = players[0].GetScore().ToString();
                Player2Name.text = players[1].NickName.ToString();
                Player2Score.text = players[1].GetScore().ToString();
            }
        }
    }
    #endregion

}
