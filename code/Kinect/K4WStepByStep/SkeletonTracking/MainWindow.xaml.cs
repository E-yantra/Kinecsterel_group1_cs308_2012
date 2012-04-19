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
//using Coding4Fun.Kinect.Wpf;
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
        SerialPort port2;
        int players = 1;
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
            if (players == 2){
                port2 = new SerialPort("COM6");

                port2.BaudRate = 9600;
                port2.Parity = Parity.None;
                port2.DataBits = 8;
                port2.Open();
                checkport2open();
            }
            
            
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            Closed += new EventHandler(MainWindow_Closed);
        }

        private void checkportopen()
        {
            if (!port.IsOpen) MessageBox.Show("Zigbee Failed to Initialize !!!");
        }
        
        private void checkport2open()
        {
            if (!port2.IsOpen) MessageBox.Show("Second Player not found !!!");
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

        void zigbee_success2(int x)
        {

            if (x == 0)
            {
                ylabel2.Content = "failed";
                ylabel2.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                ylabel2.Content = "success";
                ylabel2.Foreground = new SolidColorBrush(Colors.Green);
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
                
                numplayers.Content = (from s in allSkeletons
                                         where s.TrackingState == SkeletonTrackingState.Tracked
                                         select s).Count();
                
                Skeleton first = (from s in allSkeletons
                                         where s.TrackingState == SkeletonTrackingState.Tracked
                                         select s).FirstOrDefault();
                
                if (first == null)
                {
                    return;
                }


                if ((from s in allSkeletons
                     where s.TrackingState == SkeletonTrackingState.Tracked
                     select s).Count() == 2)
                {

                    Skeleton second = (from s in allSkeletons
                                       where s.TrackingState == SkeletonTrackingState.Tracked
                                       select s).ElementAt(1);



                    if (second != null)
                    {

                        Joint head2 = second.Joints[JointType.Head];
                        Joint lefthand2 = second.Joints[JointType.HandLeft];
                        Joint righthand2 = second.Joints[JointType.HandRight];
                        Joint rightelbow2 = second.Joints[JointType.ElbowRight];
                        Joint leftelbow2 = second.Joints[JointType.ElbowLeft];


                        int headx2 = (int)(head2.Position.X * 1000);
                        int heady2 = (int)(head2.Position.Y * 1000);
                        int rightx2 = (int)(righthand2.Position.X * 1000);
                        int righty2 = (int)(righthand2.Position.Y * 1000);
                        int leftx2 = (int)(lefthand2.Position.X * 1000);
                        int lefty2 = (int)(lefthand2.Position.Y * 1000);
                        int rightex2 = (int)(rightelbow2.Position.X * 1000);
                        int rightey2 = (int)(rightelbow2.Position.Y * 1000);
                        int leftex2 = (int)(leftelbow2.Position.X * 1000);
                        int leftey2 = (int)(leftelbow2.Position.Y * 1000);
                        string Data2;

                        if (righty2 > rightey2 + 200 && lefty2 > leftey2 + 200)
                        {
                            Data2 = "4";
                            xlabel2.Content = "Reverse";
                            //Code to Reverse
                            //result = zigbee.zgb_tx_data(Data);
                            if (players == 2)
                            {
                                try
                                {
                                    port2.Write(Data2);
                                    zigbee_success2(1);
                                }
                                catch (Exception ex)
                                {
                                    zigbee_success2(0);
                                }
                            }
                        }

                        else if (righty2 > rightey2 && rightx2 > rightex2 - 200 && rightx2 < rightex2 + 200)
                        {
                            xlabel2.Content = "Forward";
                            Data2 = "1";
                            //Code to Move Forward
                            //result = zigbee.zgb_tx_data(Data);
                            if (players == 2)
                            {
                                try
                                {
                                    port2.Write(Data2);
                                    zigbee_success2(1);
                                }
                                catch (Exception ex)
                                {
                                    zigbee_success2(0);
                                }
                            }
                        }
                        else if (rightx2 > rightex2 + 200 && leftx2 > leftex2 + 200)
                        {
                            xlabel2.Content = "Hard Right";
                            Data2 = "2";
                            //Code to Hard Right


                            //result = zigbee.zgb_tx_data(Data);
                            if (players == 2)
                            {
                                try
                                {
                                    port2.Write(Data2);
                                    zigbee_success2(1);
                                }
                                catch (Exception ex)
                                {
                                    zigbee_success2(0);
                                }
                            }
                        }
                        else if (rightx2 < rightex2 - 200 && leftx2 < leftex2 - 200)
                        {
                            Data2 = "3";
                            xlabel2.Content = "Hard Left";

                            //Code to Hard Left
                            if (players == 2)
                            {
                                try
                                {
                                    port2.Write(Data2);
                                    zigbee_success2(1);
                                }
                                catch (Exception ex)
                                {
                                    zigbee_success2(0);
                                }
                            }
                        }

                        else if (rightx2 > rightex2 + 200)
                        {
                            Data2 = "5";
                            xlabel2.Content = "Right";
                            //Code to Right
                            //result = zigbee.zgb_tx_data(Data);
                            if (players == 2)
                            {
                                try
                                {
                                    port2.Write(Data2);
                                    zigbee_success(1);
                                }
                                catch (Exception ex)
                                {
                                    zigbee_success(0);
                                }
                            }
                        }
                        else if (rightx2 < rightex2 - 200)
                        {
                            Data2 = "6";
                            xlabel2.Content = "Left";
                            //Code to Left
                            //result = zigbee.zgb_tx_data(Data);
                            if (players == 2)
                            {
                                try
                                {
                                    port2.Write(Data2);
                                    zigbee_success2(1);
                                }
                                catch (Exception ex)
                                {
                                    zigbee_success2(0);
                                }
                            }
                        }
                        else
                        {
                            Data2 = "7";
                            xlabel2.Content = "-Stop-";
                            //Code to Stop
                            //result = zigbee.zgb_tx_data(Data);
                            if (players == 2)
                            {
                                try
                                {
                                    port2.Write(Data2);
                                    zigbee_success2(1);
                                }
                                catch (Exception ex)
                                {
                                    zigbee_success2(0);
                                }
                            }
                        }
                    }

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
                Joint rightelbow = first.Joints[JointType.ElbowRight];
                Joint leftelbow = first.Joints[JointType.ElbowLeft];

                
                int headx = (int)(head.Position.X*1000);
                int heady = (int)(head.Position.Y * 1000);
                int rightx = (int)(righthand.Position.X * 1000);
                int righty = (int) (righthand.Position.Y * 1000);
                int leftx = (int) (lefthand.Position.X * 1000);
                int lefty = (int) (lefthand.Position.Y * 1000);
                int rightex = (int)(rightelbow.Position.X * 1000);
                int rightey = (int)(rightelbow.Position.Y * 1000);
                int leftex = (int)(leftelbow.Position.X * 1000);
                int leftey = (int)(leftelbow.Position.Y * 1000);
                string Data;
                
                if (righty > rightey + 200  && lefty > leftey + 200)
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

                else if(righty > rightey && rightx > rightex - 200 && rightx < rightex + 200){
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
                else if (rightx > rightex + 200 && leftx > leftex + 200)
                {
                    xlabel.Content = "Hard Right";
                    Data = "2";
                    //Code to Hard Right


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
                else if (rightx < rightex - 200 && leftx < leftex - 200)
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
                
                else if (rightx > rightex + 200)
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
                else if (rightx < rightex - 200)
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
            xlabel2.Content = "-";
            numplayers.Content = "Exit";
            ylabel.Content = "";
            ylabel2.Content = "";
            string Data = "7";
            
            try
            {
                port.Write(Data);
                //port2.Write(Data);
                //zigbee_success(1);
            }
            catch (Exception ex)
            {
                //zigbee_success(0);
            }
            //zigbee.zgb_terminate();
            port.Close();
            if (players == 2) port2.Close();
            sensor.Stop();
            
        }
    }
}
