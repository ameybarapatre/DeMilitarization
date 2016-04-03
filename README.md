# DeMilitarization

This application adds firewall rules to the system to achieve demilitarization.

The application can allow/block various protocols/services/applications over a network between windows systems that are using this application.
The application has to be run as administrator.
The Processexecute class has to be initialzed with the required netsh firewall commands to be run to block/allow  protocols /services/applications.
The system has a unique user login feature to using MySQL databse to allow only authenticated users to use the application
MySQLCon class is reponsible for the same(Change the MySQL server link from LOCALHOST to the required link).
The users connected to the demilitarized network can be seen in the text box
UDP sockets are used to communicate the information of newly connected users to the application due to absence of DTLS support in C# vanilla UDP sockets are used.
