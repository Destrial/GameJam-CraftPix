using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destrial
{
    public class GameManagerBase : MonoBehaviour
    {
        public enum GameState
        {
            InGame,
            OnMenu
        }

        public enum GamePhase
        {
            Wander,
            Asking,
            Decide
        }

        public GameState MyState;
        public GamePhase MyPhase;
        
        public static GameManagerBase Instance;
        // Start is called before the first frame update
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                // Détruit ce doublon pour préserver le singleton
                Destroy(gameObject);
                return;
            }

            // Assigne cette instance comme l'instance unique
            Instance = this;

            // Optionnel : Garde le GameManager actif lors des changements de scènes
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
