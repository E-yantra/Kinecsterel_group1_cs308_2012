using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.IO.Ports;


namespace SkeletonTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor sensor;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        SerialPort port;
        public const int DEFAULT_PORTNUM = 3;


        public MainWindow()
        {
            InitializeComponent();
            /*
            zigbee.zgb_initialize(DEFAULT_PORTNUM);
            check_zigbee_initialization();
            */
            
            port = new SerialPort("COM8");
            
            port.BaudRate = 9600;
            port.Parity = Parity.None;
            port.DataBits = 8;
            port.Open();
            checkportopen();

            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            Closed += new EventHandler(MainWindow_Closed);
        }

        private void checkportopen()
        {
            if (!port.IsOpen)
            {
                MessageBox.Show("Zigbee Failed to Initialize !!!");
            }
        }
        /*
        private void check_zigbee_initialization()
        {
            if (zigbee.zgb_initialize(DEFAULT_PORTNUM) == 0)
            {
                MessageBox.Show("Zigbee Failed to Initialize !!!");
            }
        }
        */
        void MainWindow_Closed(object sender, EventArgs e)
        {
           
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                
                sensor = KinectSensor.KinectSensors[0];
                sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
                sensor.SkeletonStream.Enable();
                sensor.Start();
            }
            else
            {
                MessageBox.Show("No Device Found !!!");
            }
        }

        void zigbee_success(int x)
        {
            
            if (x == 0)
            {
                ylabel.Content = "failed";
                ylabel.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                ylabel.Content = "success";
                ylabel.Foreground = new SolidColorBrush(Colors.Green); 
            }
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame=e.OpenSkeletonFrame())
            {
               if (skeletonFrame == null)
                {
                    return; 
                }

                
                skeletonFrame.CopySkeletonDataTo(allSkeletons);

                //get the first tracked skeleton
                Skeleton first = (from s in allSkeletons
                                         where s.TrackingState == SkeletonTrackingState.Tracked
                                         select s).FirstOrDefault();

                if (first == null)
                {
                    return;
                }



                //set scaled position
                /*
                ScalePosition(headEllipse, first.Joints[JointType.Head]);
                ScalePosition(leftEllipse, first.Joints[JointType.HandLeft]);
                ScalePosition(rightEllipse, first.Joints[JointType.HandRight]);
                */
                Joint head = first.Joints[JointType.Head];
                Joint lefthand = first.Joints[JointType.HandLeft];
                Joint righthand = first.Joints[JointType.HandRight];
                int headx = (int)(head.Position.X*1000);
                int heady = (int)(head.Position.Y * 1000);
                int rightx = (int)(righthand.Position.X * 1000);
                int righty = (int) (righthand.Position.Y * 1000);
                int leftx = (int) (lefthand.Position.X * 1000);
                int lefty = (int) (lefthand.Position.Y * 1000);
                string Data;
                int result;
                if(righty > heady && lefty > heady){
                    xlabel.Content = "Forward";
                    Data = "1";
                    //Code to Move Forward
                    //result = zigbee.zgb_tx_data(Data);
                    try
                    {
                        port.Write(Data);
                        zigbee_success(1);
                    }
                    catch (Exception ex)
                    {
                        zigbee_success(0);
                    }
                }
                else if(leftx > headx && rightx > headx + 500){
                    xlabel.Content = "Hard Right";
                    Data = "2";
                    //Code to Hard Right

                    // serial port nubmer COM 8 write
                    // 96
                    

                    //result = zigbee.zgb_tx_data(Data);
                    try
                    {
                        port.Write(Data);
                        zigbee_success(1);
                    }
                    catch (Exception ex)
                    {
                        zigbee_success(0);
                    }
                }
                else if (rightx < headx && leftx < headx - 500)
                {
                    Data = "3";
                    xlabel.Content = "Hard Left";
                    
                    //Code to Hard Left
                    try
                    {
                        port.Write(Data);
                        zigbee_success(1);
                    }
                    catch (Exception ex)
                    {
                        zigbee_success(0);
                    }
                }
                else if (rightx > headx + 500 && leftx < headx - 500)
                {
                    Data = "4";
                    xlabel.Content = "Reverse";
                    //Code to Reverse
                    //result = zigbee.zgb_tx_data(Data);
                    try
                    {
                        port.Write(Data);
                        zigbee_success(1);
                    }
                    catch (Exception ex)
                    {
                        zigbee_success(0);
                    }
                }
                else if (rightx > headx + 500)
                {
                    Data = "5";
                    xlabel.Content = "Right";
                    //Code to Right
                    //result = zigbee.zgb_tx_data(Data);
                    try
                    {
                        port.Write(Data);
                        zigbee_success(1);
                    }
                    catch (Exception ex)
                    {
                        zigbee_success(0);
                    }
                }
                else if (leftx < headx - 500)
                {
                    Data = "6";
                    xlabel.Content = "Left";
                    //Code to Left
                    //result = zigbee.zgb_tx_data(Data);
                    try
                    {
                        port.Write(Data);
                        zigbee_success(1);
                    }
                    catch (Exception ex)
                    {
                        zigbee_success(0);
                    }
                }
                else{
                    Data = "7";
                    xlabel.Content = "-Stop-";
                    //Code to Stop
                    //result = zigbee.zgb_tx_data(Data);
                    try
                    {
                        port.Write(Data);
                        zigbee_success(1);
                    }
                    catch (Exception ex)
                    {
                        zigbee_success(0);
                    }
                }
           
            }
        }
        /*
        private void ScalePosition(FrameworkElement element, Joint joint)
        {
            //convert the value to X/Y
            //Joint scaledJoint = joint.ScaleTo(1280, 720); 

            //convert & scale (.3 = means 1/3 of joint distance)
            Joint scaledJoint = joint.ScaleTo(350, 350, .3f, .3f);

            Canvas.SetLeft(element, scaledJoint.Position.X);
            Canvas.SetTop(element, scaledJoint.Position.Y);

        }
        */
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            xlabel.Content = "-";
            ylabel.Content = "";
            //zigbee.zgb_terminate();
            port.Close();
            sensor.Stop();
        }
    }
}
