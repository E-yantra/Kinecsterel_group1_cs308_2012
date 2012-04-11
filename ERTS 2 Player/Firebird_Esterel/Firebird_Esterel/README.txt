
Assignment: Sample code for obstacle avoidance is given in Obstacle_Avoidance.strl. You have to implement esterel code for Adaptive Cruise Control. 


To run your program:
1. Check or Install FileZilla and PuTTY on your system
2. Run PuTTY and connect to hostname
	hostname - ertslab@10.129.11.205
	password - firebird
3. Go to folder Desktop/ and create your group folder Group_(Team No) example Group_4/
4. Now run FileZilla
	host - 10.129.11.205
	username - ertslab
	password - firebird
	port - 22
5. Now transfer these files to your Group folder
	buildhash.pl, firebird_gen, firebird_winavr.h, getmodulename.pl, [you_esterel_file_name].strl
6. Now went back to PuTTY and run the following command
	./firebird_gen [you_esterel_file_name].strl
	NOTE: You will get error like pdg: unexpected operator. Ignore this error, rest you have to fix your own
7. [you_esterel_file_name].c is generated in same folder transfer that file to your machine using FileZilla
8. Compile it with AVR Studio and use BootLoader to burn it on Firebird

NOTE: After completing the assignment please delete your folder from server 

