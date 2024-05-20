using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaCat_Task
{
    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    public class Singleton : MonoBehaviour
    {
        public static Singleton Instance { get; private set; }

        [SerializeField] LevelManager levelManager;
        [SerializeField] UIElements uIElements;
        [SerializeField] GridGenerator gridGenerator;
        public LevelManager Level_Manager { get => levelManager; }
        public UIElements UI_Elements { get => uIElements; }

        public GridGenerator Grid_Generator { get => gridGenerator; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
           
        }
    }
}