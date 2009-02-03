using VIMControls.Contracts;

namespace VIMControls.Controls.VIMForms
{
    public class VIMMultilineConstraint : IVIMFormConstraint
    {
        public bool Multiline
        {
            get { return true; }
        }
    }

    public class VIMDefaultConstraint : IVIMFormConstraint
    {
        public bool Multiline
        {
            get { return false; }
        }
    }

    public class VIMFormConstraint
    {
        public static IVIMFormConstraint Multiline = new VIMMultilineConstraint();
        public static IVIMFormConstraint Default = new VIMDefaultConstraint();
    }
}