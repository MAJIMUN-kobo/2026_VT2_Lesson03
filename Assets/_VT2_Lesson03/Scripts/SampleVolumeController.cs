using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SampleVolumeController : MonoBehaviour
{
    [SerializeField] private Volume _globalVolume;

    [Header("=== パラメーター ===")]
    public float Hp;
    public float MaxHp;     // 体力値
    public float HpRate;    // 体力率

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 無限回復
        OnHeal(0.5f);

        // スペースキーを押したら体力を減らす
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnDamage(50f);
        }

        if(_globalVolume != null)
        {
            // GlobalVolume のプロファイルから[Vignette]コンポーネントを取得する
            if ( _globalVolume.profile.TryGet(out Vignette vignette) )
            {
                vignette.center.overrideState = true;
                vignette.center.value = Camera.main.WorldToViewportPoint(transform.position); 
                
                vignette.intensity.overrideState = true;
                //vignette.intensity.value = 1 - (HpRate);
                vignette.intensity.value = Mathf.Lerp( vignette.intensity.value, 1 - ( HpRate ), Time.deltaTime * 5f );

                vignette.color.overrideState = true;
                vignette.color.value = new Color(1f, 0f, 0f);
            }

            // 自作のボリュームを取得していじれる。
            if( _globalVolume.profile.TryGet(out SamplePostProcessingVolumeComponent sample))
            {
                sample.intensity.overrideState = true;
                // sample.intensity.value = Mathf.PingPong(Time.time, 1.0f);
            }
        }
    }

    // === 体力を減らすメソッド ===
    public void OnDamage(float damage)
    {
        // 体力を減らす
        Hp -= damage;
        Hp = Mathf.Clamp(Hp, 0, MaxHp);

        // 体力率を計算する
        HpRate = Hp / MaxHp;
    }

    // === 体力を回復するメソッド ===
    public void OnHeal(float heal)
    {
        // 体力を増やす
        Hp += heal;
        Hp = Mathf.Clamp(Hp, 0, MaxHp);

        // 体力率を計算する
        HpRate = Hp / MaxHp;
    }
}
