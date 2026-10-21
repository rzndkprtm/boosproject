<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Specs_Default" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>OneDrive Documents</title>

    <style>
        body {
            font-family: Arial, sans-serif;
            background: #f5f7fa;
            margin: 0;
            padding: 50px 20px;
            text-align: center;
        }

        .container {
            max-width: 600px;
            margin: auto;
            padding: 40px;
            background: white;
            border-radius: 10px;
        }

        .btn {
            display: inline-block;
            padding: 14px 25px;
            background: #0078d4;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
        }

        .btn:hover {
            background: #106ebe;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <div class="container">
            <h2>OneDrive Documents</h2>

            <p>
                Klik tombol di bawah untuk melihat
                folder dokumen.
            </p>

            <a class="btn"
               href="https://onedrive.live.com/?id=%2Fpersonal%2Fae9316f9f664f948%2FDocuments%2FSPEED%20OneDrive%2Fno%20logo%20specs&amp;listurl=%2Fpersonal%2Fae9316f9f664f948%2FDocuments&amp;e=q6bhMl&amp;migratedtospo=true&amp;redeem=aHR0cHM6Ly8xZHJ2Lm1zL3UvcyFBa2o1WlBiNUZwT3VoemFHX2ZQQ0VrSWlqeW5kP2U9cTZiaE1s&amp;ga=1"
               target="_blank"
               rel="noopener noreferrer">
                Buka Folder OneDrive
            </a>
        </div>

    </form>
</body>
</html>