namespace HoweFramework
{
    /// <summary>
    /// 实例引用。用于存储一个带实例id的引用，当其实例id发生变化时，会返回null给外部。
    /// </summary>
    public readonly struct ReferenceRef<T> where T : IReferenceWithId
    {
        /// <summary>
        /// 原始引用实例。
        /// </summary>
        private readonly T m_Reference;

        /// <summary>
        /// 原始引用实例的实例id。
        /// </summary>
        private readonly int m_InstanceId;

        public ReferenceRef(T reference)
        {
            m_Reference = reference;
            m_InstanceId = reference.InstanceId;
        }

        /// <summary>
        /// 获取原始引用实例。
        /// </summary>
        public T Reference => m_Reference?.InstanceId != m_InstanceId ? default : m_Reference;

        /// <summary>
        /// 隐式转换为原始引用实例。
        /// </summary>
        public static implicit operator T(ReferenceRef<T> referenceRef)
        {
            return referenceRef.Reference;
        }

        /// <summary>
        /// 隐式转换为实例引用。
        /// </summary>
        public static implicit operator ReferenceRef<T>(T reference)
        {
            return new ReferenceRef<T>(reference);
        }
    }
}