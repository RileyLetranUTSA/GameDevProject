using UnityEngine;

public abstract class BossBase : MonoBehaviour
{
    protected BossController controller;

    public void Init(BossController bossController)
    {
        controller = bossController;
    }

    public abstract void CheckToFire(ref float nextShootTime);
    public abstract void EnterBasePhase(int phaseNumber);
    public abstract void EnterCharmPhase(int phaseNumber);
}
