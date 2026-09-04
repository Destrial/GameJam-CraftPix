using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Destrial
{
    public class PlayerController2DBase : MonoBehaviour
    {
     

        // Public variables
        public float speed = 5f; // The speed at which the player moves
        public float rotationSpeed = 5f;
     
        [SerializeField] private Transform spriteTransform;

        // Private variables 
        BasicInput _myInput;

        private Rigidbody2D rb; // Reference to the Rigidbody2D component attached to the player
        private Vector2 movement; // Stores the direction of player movement

        
        //private bool moving = false;    //check if the player is in movement or not (Currently useless)

       

        private void Awake()
        {
            _myInput = new BasicInput();
        }

        private void OnEnable()
        {
            _myInput.Enable();
            
        }

      
        private void OnDisable()
        {
            _myInput.Disable();
        
        }

        // Start is called before the first frame update
        void Start()
        {
          
            // Initialize the Rigidbody2D component
            rb = GetComponent<Rigidbody2D>();
        
            // Prevent the player from rotating
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        void Update()
        {
        }

        void FixedUpdate() //MVT
        {
            if (GameManagerBase.Instance.MyState != GameManagerBase.GameState.OnMenu)
            {
                Vector2 moveInput = _myInput.Player.Move.ReadValue<Vector2>();

                // 1. D�placement physique du Rigidbody
                Vector2 targetDisplacement = moveInput * speed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + targetDisplacement);
                float smoothedAngle;
                float targetAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
                smoothedAngle = Mathf.MoveTowardsAngle(spriteTransform.eulerAngles.z, targetAngle,
                    rotationSpeed * Time.fixedDeltaTime);

                // 2. Gestion Visuelle (Sprite)
                if (moveInput.sqrMagnitude > 0.01f)
                {
                    // Flip horizontal sur le sprite enfant
                    float scaleX = moveInput.x < -0.05f
                        ? -1f
                        : (moveInput.x > 0.05f ? 1f : spriteTransform.localScale.x);
                    spriteTransform.localScale = new Vector3(scaleX, 1f, 1f);


                    // Ajustement si le sprite regarde � gauche
                    if (scaleX < 0)
                    {
                        targetAngle += 180f;
                    }

                    // Lissage et application de la rotation uniquement sur l'enfant
                    smoothedAngle = Mathf.MoveTowardsAngle(spriteTransform.eulerAngles.z, targetAngle,
                        rotationSpeed * Time.fixedDeltaTime);
                }

                spriteTransform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
            }
        }


    }
}
