using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Manager {
     public class GameManager : MonoBehaviour {
        private static GameManager _intance;
        public static GameManager Instance => _intance;
        [Header("Ammo Prefabs")] 
        public AmmoScript catAmmo;
        public AmmoScript dogAmmo;
        [Space] [Header("Aiming arrow in scene")]
        public Transform aimArrow;

        [Space] [Header("Position of Characters")]
        public Transform playerOnePlace;
        public Transform playerTwoPlace;
        [Header("Character prefabs")] 
        public GameObject catPrefab;
        public GameObject dogPrefab;
        [Header("Game mode flags")]
        public StartingFlagsSO startingFlags;
        private bool _playerVsPlayer = false;
        private bool _playerIsCat = true;

        private List<PlayerController> _controllers;
        //State
        private GameState _state;
        private bool _clickSensitive = false;
        private PlayerController _activePlayer = null;
        
        //Shooting
        private Vector2 _direction = Vector2.zero;
        private double _force = 10;
        private double _degree = 15;
        private bool _raiseForce = true;
        [Header("Radius around character for aim")]
        public double r = 50;
        private bool _raiseDeg = true;
        private SliderController _activeSliderController;
        
        //Animators
        private Animator _oneAnim;
        private Animator _twoAnim;
        
        //ForceBars
        [Header("Force Bars")]
        public GameObject forceBarOne;
        public GameObject forceBarTwo;
        private SliderController _sliderControllerOne;
        private SliderController _sliderControllerTwo;
        public float minForce = 10;
        public float maxForce = 15;

        //HealthBars
        [Header("Health System")]
        public Image leftImage;
        public Image rightImage;
        public SliderController leftHealth;
        public SliderController rightHealth;
        public Sprite catImage;
        public Sprite dogImage;
        public int maxHeath = 100;

        //WindPower
        [Header("Wind System")]
        public SliderController windPowerSlider;
        public Image windDirection;
        private float _windPower;
        private bool _windLeft;
        
        //CharacterButtons
        [Header("Character Buttons and Arrow")] 
        public Button leftButton;
        public Button rightButton;
        public GameObject characterArrow;
        
        //SpeciamAmmo
        [Header("Special Ammo")] 
        public GameObject leftPanel;
        public GameObject rightPanel;
        [Space]
        public AmmoScript catBigAmmo;
        public AmmoScript dogBigAmmo;
        private AmmoScript _playerOneBigAmmo;
        private AmmoScript _playerTwoBigAmmo;
        private Button _lastClicked = null;
        public LifeStealAmmoScript lifeStealAmmo;
        public StunAmmoScript stunAmmo;

        public Vector2 Wind => (_windLeft ? Vector2.left : Vector2.right) * _windPower;

        private void SpawnAndSetCharacters() {
            GameObject cat = Instantiate(catPrefab);
            GameObject dog = Instantiate(dogPrefab);

            Destroy(playerOnePlace.gameObject);
            Destroy(playerTwoPlace.gameObject);

            var position1 = playerOnePlace.position;
            var position2 = playerTwoPlace.position;
            
            cat.transform.position = _playerVsPlayer || _playerIsCat ? position1 : position2;
            dog.transform.position = _playerVsPlayer || _playerIsCat ? position2 : position1;

            if (_playerVsPlayer) {
                _controllers.Add(cat.AddComponent(typeof(PlayerController)) as PlayerController);
                _controllers.Add(dog.AddComponent(typeof(PlayerController)) as PlayerController);

                _controllers[0].Ammo = catAmmo;
                _controllers[1].Ammo = dogAmmo;
                _controllers[1].Left = false;
                dog.GetComponent<SpriteRenderer>().flipX = true;
            } else if (_playerIsCat) {
                _controllers.Add(cat.AddComponent(typeof(PlayerController)) as PlayerController);
                _controllers.Add(dog.AddComponent(typeof(AIController)) as PlayerController);
                _controllers[0].Ammo = catAmmo;
                _controllers[1].Ammo = dogAmmo;
                dog.GetComponent<SpriteRenderer>().flipX = true;
            }
            else {
                _controllers.Add(dog.AddComponent(typeof(PlayerController)) as PlayerController);
                _controllers.Add(cat.AddComponent(typeof(AIController)) as PlayerController);
                _controllers[0].Ammo = dogAmmo;
                _controllers[1].Ammo = catAmmo;
                cat.GetComponent<SpriteRenderer>().flipX = true;
            }
            _controllers[0].Left = true;
            _controllers[1].Left = false;

            _oneAnim = _playerVsPlayer || _playerIsCat ? cat.GetComponent<Animator>() : dog.GetComponent<Animator>();
            _twoAnim = _playerVsPlayer || _playerIsCat ? dog.GetComponent<Animator>() : cat.GetComponent<Animator>();
        }

        private void Awake() {
            _playerIsCat = startingFlags.playerIsCat;
            _playerVsPlayer = startingFlags.playerVsPlayer;
            leftPanel.SetActive(true);
            rightPanel.SetActive(false);
            
            _controllers = new List<PlayerController>(2);
            _intance = this;
           
            SpawnAndSetCharacters();
            
            _state = GameState.IdleOne;
            _activePlayer = _controllers[0];
            aimArrow.gameObject.SetActive(false);

            _sliderControllerOne = forceBarOne.GetComponentInChildren<SliderController>();
            _sliderControllerTwo = forceBarTwo.GetComponentInChildren<SliderController>();

            leftImage.sprite = _playerIsCat || _playerVsPlayer ? catImage : dogImage;
            rightImage.sprite = _playerIsCat || _playerVsPlayer ? dogImage : catImage;
            rightImage.transform.rotation = Quaternion.AngleAxis(180, new Vector3(0, 1, 0));
            _controllers.ForEach(c => c.Health = maxHeath);
            _controllers.ForEach(c => c.HealthChanged += RefreshHelthBars);
            
            rightButton.interactable = false;
            characterArrow.transform.position = _controllers[0].transform.position + new Vector3(0, 1.5f, 0);

            _playerOneBigAmmo = _playerIsCat || _playerVsPlayer ? catBigAmmo : dogBigAmmo;
            _playerTwoBigAmmo = _playerIsCat || _playerVsPlayer ? dogBigAmmo : catBigAmmo;
        }

        private void Start() {
            RefreshWindPower();
        }

        public void DoAction() {
            GameState nextState = _state;
            switch (_state) {
                case GameState.IdleOne:
                    _clickSensitive = true;
                    _oneAnim.Play("PrepareShoot");
                    nextState = GameState.AimOne;
                    leftButton.interactable = false;
                    characterArrow.gameObject.SetActive(false);
                    _lastClicked = null;
                    leftPanel.SetActive(false);
                    break;
                
                case GameState.AimOne:
                    _clickSensitive = true;
                    forceBarOne.SetActive(true);
                    nextState = GameState.ForceOne;
                    _activeSliderController = _sliderControllerOne;
                    break;
                
                case GameState.ForceOne:
                    _clickSensitive = false;
                    _oneAnim.Play("Shoot");
                    aimArrow.gameObject.SetActive(false);
                    forceBarOne.SetActive(false);
                    _activePlayer.Shoot((aimArrow.position - _controllers[0].transform.position).normalized * (float) _force);
                    nextState = GameState.ShootOne;
                    break;
                
                case GameState.ShootOne:
                    _clickSensitive = false;
                    if (_playerVsPlayer) {
                        nextState = GameState.IdleTwo;
                        rightButton.interactable = true;
                        characterArrow.gameObject.SetActive(true);
                        characterArrow.transform.position = _controllers[1].transform.position + new Vector3(0, 1.5f, 0);
                        rightPanel.SetActive(true);
                    }
                    else {
                        nextState = GameState.AITurn;
                        _clickSensitive = false;
                        if (_controllers[1].Health != 0)
                          _controllers[1].takeTurn();
                    }
                    _activePlayer = _controllers[1];
                    break;
                
                case GameState.IdleTwo:
                    _clickSensitive = true;
                    rightButton.interactable = false;
                    characterArrow.gameObject.SetActive(false);
                    _twoAnim.Play("PrepareShoot");
                    nextState = GameState.AimTwo;
                    _lastClicked = null;
                    rightPanel.SetActive(false);
                    break;
                
                case GameState.AimTwo:
                    _clickSensitive = true;
                    forceBarTwo.SetActive(true);
                    _activeSliderController = _sliderControllerTwo;
                    nextState = GameState.ForceTwo;
                    break;
                
                case GameState.ForceTwo:
                    _clickSensitive = false;
                    _twoAnim.Play("Shoot");
                    _activePlayer.Shoot((aimArrow.position - _controllers[1].transform.position).normalized * (float) _force);
                    aimArrow.gameObject.SetActive(false);
                    forceBarTwo.SetActive(false);
                    nextState = GameState.ShootTwo;
                    break;
               
                case GameState.ShootTwo:
                    _clickSensitive = false;
                    leftButton.interactable = true;
                    characterArrow.gameObject.SetActive(true);
                    characterArrow.transform.position = _controllers[0].transform.position + new Vector3(0, 1.5f, 0);
                    _activePlayer = _controllers[0];
                    nextState = GameState.IdleOne;
                    leftPanel.SetActive(true);
                    RefreshWindPower();
                    break;
                
                case GameState.AITurn:
                    _clickSensitive = false;
                    nextState = GameState.ShootTwo;
                    break;
            }

            _state = nextState;
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0) && _clickSensitive) {
                DoAction();
            }
        }

        private void FixedUpdate() {
            if (_state == GameState.AimOne || _state == GameState.AimTwo) {
                if (_degree > 90)
                    _raiseDeg = false;
                if (_degree < 15)
                    _raiseDeg = true;
                _degree += _raiseDeg ? 2 : -2;
                aimArrow.position = _activePlayer.GetAimPos(_degree, r);
                if (_state == GameState.AimOne) 
                    aimArrow.rotation = Quaternion.AngleAxis((float) (_degree - 90), new Vector3(0, 0, 1));
                if (_state == GameState.AimTwo)
                    aimArrow.rotation = Quaternion.AngleAxis((float) (_degree), new Vector3(0, 0, 1));
                if (!aimArrow.gameObject.active)
                    aimArrow.gameObject.SetActive(true);
            }

            if (_state == GameState.ForceOne || _state == GameState.ForceTwo) {
                if (_force < minForce)
                    _raiseForce = true;
                if (_force > maxForce)
                    _raiseForce = false;
                _force += _raiseForce ? 0.2 : -0.2;

                _activeSliderController.Slider = (float) ((maxForce - _force) / (maxForce - minForce));
            }
        }

        private void RefreshHelthBars() {
            leftHealth.Slider = (float) _controllers[0].Health / maxHeath;
            rightHealth.Slider = (float) _controllers[1].Health / maxHeath;
            if (_controllers[0].Health == 0) {
                _oneAnim.Play("Death");
                _state = GameState.End;
            }
            if (_controllers[1].Health == 0) {
                _twoAnim.Play("Death");
                _state = GameState.End;
            }
        }

        private void RefreshWindPower() {
            float min = 0;
            float max = minForce / 2;
            _windLeft = Random.value < 0.5;
            _windPower = Random.Range(min, max);

            windPowerSlider.Slider = (_windPower - min) / (max - min);
            windDirection.gameObject.transform.rotation = Quaternion.AngleAxis(_windLeft ? 180 : 0, new Vector3(0, 1, 0));
        }

        public void BigAmmoButton(Button button) {
            if (!(_lastClicked is null))
                _lastClicked.interactable = true;
            _lastClicked = button;
            button.interactable = false;
            if (_state == GameState.IdleOne)
                _controllers[0].NextSpec = _playerOneBigAmmo;
            else if (_state == GameState.IdleTwo)
                _controllers[1].NextSpec = _playerTwoBigAmmo;
        }
        
        public void LifeStealButton(Button button) {
            if (!(_lastClicked is null))
                _lastClicked.interactable = true;
            _lastClicked = button;
            button.interactable = false;
            _activePlayer.NextSpec = lifeStealAmmo;
        }
        
        public void StunButton(Button button) {
            if (!(_lastClicked is null))
                _lastClicked.interactable = true;
            _lastClicked = button;
            button.interactable = false;
            _activePlayer.NextSpec = stunAmmo;
        }

        public void Stun() {
            if (_state == GameState.ShootOne)
                _state = GameState.ShootTwo;
            else if (_state == GameState.ShootTwo)
                _state = GameState.ShootOne;
            DoAction();
        }
     }   
}