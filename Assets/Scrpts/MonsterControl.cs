using UnityEngine;
using System.Collections;

public class MonsterControl : MonoBehaviour {

	//게임메니저에서 몬스터를 컨트롤 한다.
	public In_GameManager mIn_GameManager;
	
	//mAnimator를 선언.
	private Animator mAnimator;

	//몬스터 숫자.
	public int idx;
	
	//몬스터 의 공격력, 채력, 공격 속도에 사용 될 변수.
	// HP
	public int mOrinHP;
	[HideInInspector]
	public int mHP;
	
	//MANA
	public int mOrinMP;
	[HideInInspector]
	public int mMP;
	
	//공격 데미지 (현제는 걍 데미지만.. 나중에 공격력이랑 수정 필요)
	public int mOrinAttack;
	[HideInInspector]
	public int mAttack;
	
	//공속 
	public float mAttackSpeed;
	
	//추후 스킬 구현..스킬 리스트.??
	
	//히어로 의 상태 (대기, 달림, 공격, 사망)
	public enum Status
	{
		Idle,
		Attack,
		Damaged,
		UseSkill,
		Dead
	}
	
	//public으로 선언 되었지만, 인스팩터 뷰에서 노출되지 않기를 원하는 경우 
	//hideinspector를 선언합니다.
	[HideInInspector]
	public Status mStatus = Status.Idle; //히이로의 기본상태를 idle로 설정.
	
	
	
	
	// Use this for initialization
	void Start () {
		
		//1.HP 넣고, 2. 백그라운드 컴퍼넌트 넣고, 3. 활이 발사될 장소를 넣고. 스타트
		mHP = mOrinHP;
		mAttack = mOrinAttack;
		
		//Archer의 Animator 컴포넌트 레퍼런스를 가져옵니다.
		//이 script가 붙은 gameObject에 Animator를 가져옴.
		mAnimator = gameObject.GetComponent<Animator> ();
		
		
	}

	//상테와 파라메터를 통해 아처의 상태를 컨트롤 합니다.
	public void SetStatus(Status status)
	{
		//animator 에서 만든 상태 간 전이를 상황에 맞게 호출 한다.
		switch (status) {
		case Status.Idle:
			mAnimator.SetTrigger("Idle");
			Debug.Log("MM idle---");
			break;
			
		case Status.Attack:
			mAnimator.SetTrigger("Attack");
			Debug.Log("MM Attack---");
			break;
			
		case Status.Dead:
			mAnimator.SetTrigger("Dead");
			Debug.Log("MM Die---");
			break;
			
		case Status.Damaged:
			mAnimator.SetTrigger("Damaged");
			Debug.Log("MM Damage---");
			break;
			
		case Status.UseSkill:
			mAnimator.SetTrigger("Skill");
			Debug.Log("MMSkill---");
			break;
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RandomHP()
	{
		mHP += Random.Range(-10, 10);    
	}


	public void Damaged()
	{
		Debug.Log ("monster hitted");
		
		GameObject Hero = GameObject.Find ("Hero");
		HeroControl archercontrol = Hero.GetComponent<HeroControl> ();
		
		//mHP -= archercontrol.GetRandomDamage ();
		mAnimator.SetTrigger ("Damage");
		
		
		// 사망처리
		if(mHP <= 0)
		{
			mStatus = Status.Dead;
			mHP = 0;
			//mCollider.enabled = false;
			mAnimator.SetTrigger("Dead");
			//mGameManager.ReAutoTarget();
			Destroy(gameObject, 1f);
		}
	}





































}
