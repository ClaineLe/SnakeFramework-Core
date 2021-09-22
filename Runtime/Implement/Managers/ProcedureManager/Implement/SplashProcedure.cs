using com.snake.framework.runtime;

namespace com.snake.framework
{
    /// <summary>
    /// ÉÁÆÁÁ÷³Ì
    /// </summary>
    internal class SplashProcedure : BaseProcedure
    {
        protected override void onTick(ProcedureManager owner, int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
        {
            owner.SwitchProcedure<PreloadProcedure>();
        }
    }
}
