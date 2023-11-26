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
    [SerializeField] private Button _collectButton;
    [SerializeField] private GameObject _rewardsHidePositions;
    

    private Vector3 _rewardStartPosition;
    private List<GameObject> _rewards = new();

    public static ChestController Instance {get; private set;}
    private void Awake()
    {    
        Instance = this;
        _collectButton.gameObject.SetActive(false);
        _collectButton.onClick.AddListener(RewardHide);
    } 

    public void ReceiveReward(List<Reward> rewards)
    {
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
        Animator chestAnimator = chestGameObject.GetComponent<Animator>();
        chestAnimator.SetTrigger("Show");
        Chest chest=chestGameObject.GetComponent<Chest>();
        _rewardStartPosition = chest.RewardsStartPosition.position; 
        chest.OnChestOpen+=RewardsAppear;
    }

    private async void RewardsAppear()
    {
        float delay = 0.5f;
        foreach(GameObject reward in _rewards)        
        {
            Vector3 finishPosition=new Vector3(reward.transform.position.x,reward.transform.position.y,_rewardStartPosition.z);
            reward.transform.SetParent(_RewardsContainer.transform);
            reward.transform.position=_rewardStartPosition;
            reward.transform.localScale=Vector3.zero;
            reward.transform.DOScale(1,delay);
            reward.transform.DOMove(finishPosition, delay).SetEase(Ease.OutQuad);

        }
        await Task.Delay((int)(delay*1000f));
        _collectButton.gameObject.SetActive(true);
    }

    private async void RewardHide()
    {
        float delay = 2.5f;
        foreach(GameObject reward in _rewards)        
        {           
            reward.transform.DOMoveY(_rewardsHidePositions.transform.position.y, delay).SetEase(Ease.OutQuad);
            reward.transform.DOScale(0,delay/2f);
        }        
        _collectButton.gameObject.SetActive(false);   
        await Task.Delay(1000);
        _testButtonsContainer.SetActive(true);
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
            rewardObject.GetComponent<RewardPrefab>().Init(reward.Amount);
            _rewards.Add(rewardObject);
        }

    }


}
