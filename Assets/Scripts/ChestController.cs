using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

public class ChestController : MonoBehaviour
{
    [SerializeField] private GameObject _testButtonsContainer;
    [SerializeField] private GameObject _chestPrefab;
    [SerializeField] private List<RewardPrefab> _rewardPrefabs;
    [SerializeField] private GameObject _iconsPositions_01_04;
    [SerializeField] private GameObject _iconsPositions_05_08;
    [SerializeField] private GameObject _RewardsContainer;
    [SerializeField] private Animator _collectButton;
    [SerializeField] private Transform _rewardsHidePositions;
    [SerializeField] private Transform _rewardsWayPoint;
    [SerializeField] private Transform _coinTargetPosition;
    [SerializeField] private ParticleSystem _coinFinish;
    [SerializeField] private Transform _positionCounter;
    [SerializeField] private Transform _wayPoint;
    [SerializeField] private Transform _chestContainer;

    private Vector3 _rewardStartPosition;
    private List<RewardPrefab> _rewards = new();
    private Vector3 _oldPos;
    private Animator _chestAnimator;    

    public static ChestController Instance {get; private set;}
    private void Awake()
    {    
        _oldPos=_positionCounter.localPosition;    
        Instance = this;
        _collectButton.gameObject.SetActive(false);
        _collectButton.GetComponent<Button>().onClick.AddListener(RewardHide);
    } 

    public void ReceiveReward(List<Reward> rewards)
    {
        _positionCounter.localPosition=new Vector3(_oldPos.x,_oldPos.y+180,_oldPos.z);
        _testButtonsContainer.SetActive(false);
        Debug.Log("ReceiveRewards");
        foreach(var reward in rewards)
        {
            Debug.Log(reward.RewardType + " " + reward.Amount); 
        }

        FindIconsPositions(rewards);
        SpawnChest();       
       
    }

    private void SpawnChest()
    {
         _collectButton.gameObject.SetActive(false);
        GameObject chestGameObject = Instantiate(_chestPrefab, transform.position, Quaternion.identity, transform);
        chestGameObject.transform.SetParent(_chestContainer);
        _chestAnimator = chestGameObject.GetComponent<Animator>();
        _chestAnimator.SetTrigger("Show");
        Chest chest=chestGameObject.GetComponent<Chest>();
        _rewardStartPosition = chest.RewardsStartPosition.position; 
        chest.OnChestOpen+=RewardsAppear;
    }

    private async void RewardsAppear()
    {
        float delay = 0.5f;
        foreach(var reward in _rewards)        
        {
            Vector3 finishPosition=new Vector3(reward.transform.position.x,reward.transform.position.y,_rewardStartPosition.z);
            reward.transform.SetParent(_RewardsContainer.transform);
            reward.transform.position=_rewardStartPosition;
            reward.transform.localScale=Vector3.zero;
            reward.transform.DOScale(1,delay);
            reward.transform.DOMove(finishPosition, delay).SetEase(Ease.OutQuad);
            reward.Trail.gameObject.SetActive(true);

        }
        await Task.Delay(850);
        _collectButton.gameObject.SetActive(true);
    }

    private async void RewardHide()
    {
        _chestAnimator.SetTrigger("Hide");
        foreach(var reward in _rewards)        
        {    
            if (reward.RewardType==RewardType.Coin) 
            {
               FlyCoin(reward);
            }
            else 
            {
                float flyTime = 2.5f;  
                float delay=Random.Range(0.2f, 0.8f);          

                Vector3[] points=
                {
                    _rewardsWayPoint.position, _rewardsHidePositions.position
                };
                reward.transform.DOPath(points, flyTime, PathType.CatmullRom).SetEase(Ease.OutQuad).SetDelay(delay);
                reward.transform.DOScale(1,flyTime/2f).SetDelay(delay);

            }

            
        }    
        _collectButton.SetTrigger("hide");
        await Task.Delay(1000);   
        _collectButton.gameObject.SetActive(false);           
        _testButtonsContainer.SetActive(true);
    }

    private async void FlyCoin(RewardPrefab reward)
    {
        _positionCounter.DOLocalMoveY(_oldPos.y,0.2f);
        await Task.Delay(200);
        Vector3[] points=
        {
             _wayPoint.position, _coinTargetPosition.position
        };
        reward.transform.DOPath(points, 1.6f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(()=>OnCoinFinish(reward));

    }

    private void OnCoinFinish (RewardPrefab reward)
    {
        reward.gameObject.SetActive (false);
        _coinFinish.Play ();
    }

    private void FindIconsPositions(List<Reward> rewards)
    {
        foreach (Transform child in _iconsPositions_01_04.transform) 
            Destroy(child.gameObject); 

        foreach (Transform child in _iconsPositions_05_08.transform) 
            Destroy(child.gameObject);

        foreach (Transform child in _RewardsContainer.transform) 
            Destroy(child.gameObject);

        _rewards.Clear();
        
        if (rewards.Count>4)
        {
            _iconsPositions_05_08.SetActive(true);
        }
        else
        {
            _iconsPositions_05_08.SetActive(false);
        }

        for (int i=0; i<rewards.Count; i++)
        {
            var reward = rewards[i];
            var rewardPrefab = _rewardPrefabs.Find(prefab => prefab.RewardType == reward.RewardType);
            GameObject rewardObject;

            if (i<4)
            {
                rewardObject = Instantiate(rewardPrefab.gameObject, Vector3.zero, Quaternion.identity, _iconsPositions_01_04.transform);  
            } 
            else 
            {
                rewardObject = Instantiate(rewardPrefab.gameObject, Vector3.zero, Quaternion.identity, _iconsPositions_05_08.transform);  
            }
            RewardPrefab r=rewardObject.GetComponent<RewardPrefab>();
            r.Init(reward.Amount);
            _rewards.Add(r);
        }

    }


}
