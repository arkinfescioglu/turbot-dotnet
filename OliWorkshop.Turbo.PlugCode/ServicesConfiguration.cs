namespace OliWorkshop.Turbo.PlugCode
{
    public interface ServicesConfiguration<TContainer>
    {
        void ConfigureService(TContainer container);
    }
}