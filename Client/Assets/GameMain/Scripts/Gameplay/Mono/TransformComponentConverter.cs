namespace GameMain
{
    /// <summary>
    /// 变换组件转换器。
    /// </summary>
    public sealed class TransformComponentConverter : GameComponentConverterBase<TransformComponent>
    {
        /// <summary>
        /// 转换组件。
        /// </summary>
        /// <param name="entity">实体。</param>
        public override void Convert(IGameEntity entity)
        {
            var currentTransform = transform;

            var component = entity.AddComponent<TransformComponent>();
            component.Position = currentTransform.localPosition;
            component.EulerAngles = currentTransform.localEulerAngles;
            component.Scale = currentTransform.localScale;
        }
    }
}