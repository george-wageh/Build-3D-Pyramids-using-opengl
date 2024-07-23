using System.Windows.Forms;
using System.Threading;
namespace Graphics
{
    public partial class GraphicsForm : Form
    {
        Renderer renderer = new Renderer();
        Thread MainLoopThread;
        public GraphicsForm()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();
            initialize();
            MainLoopThread = new Thread(MainLoop);
            MainLoopThread.Start();

        }
        void initialize()
        {
            renderer.Initialize();   
        }
        
        void MainLoop()
        {
            while (true)
            {
                renderer.Update();
                renderer.Draw();
                simpleOpenGlControl1.Refresh();
            }
        }
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer.CleanUp();
            MainLoopThread.Abort();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            renderer.Draw();

        }

        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            float speed = 0.2f;
            if (e.KeyChar == 'f') {
                renderer.translationX = 0;
                renderer.translationY = 0;
                renderer.translationZ = 0;
                renderer.selected_id = (renderer.selected_id+1)%3;
            }

            if (e.KeyChar == 'd')
                renderer.translationX += speed;
            if (e.KeyChar == 'a')
                renderer.translationX -= speed;

            if (e.KeyChar == 'w')
                renderer.translationY += speed;
            if (e.KeyChar == 's')
                renderer.translationY -= speed;

            if (e.KeyChar == 'z')
                renderer.translationZ += speed;
            if (e.KeyChar == 'c')
                renderer.translationZ -= speed;

            simpleOpenGlControl1.Refresh();
        }
    }
}
