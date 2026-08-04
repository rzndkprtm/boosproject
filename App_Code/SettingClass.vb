Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.Cryptography

Public Class SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetDataRow(thisString As String) As DataRow
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim thisTable As New DataTable()
                        thisAdapter.Fill(thisTable)
                        If thisTable.Rows.Count > 0 Then
                            Return thisTable.Rows(0)
                        Else
                            Return Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetDataRowSP(spName As String, params As List(Of SqlParameter)) As DataRow
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(spName, thisConn)
                    thisCmd.CommandType = CommandType.StoredProcedure
                    thisCmd.Parameters.AddRange(params.ToArray())
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim thisTable As New DataTable()
                        thisAdapter.Fill(thisTable)
                        If thisTable.Rows.Count > 0 Then
                            Return thisTable.Rows(0)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return Nothing
    End Function

    Public Function GetDataTable(thisString As String) As DataTable
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim thisTable As New DataTable()
                        thisAdapter.Fill(thisTable)
                        Return thisTable
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetDataTableSP(spName As String, params As List(Of SqlParameter)) As DataTable
        Dim thisTable As New DataTable()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(spName, thisConn)
                    thisCmd.CommandType = CommandType.StoredProcedure
                    If params IsNot Nothing AndAlso params.Count > 0 Then
                        thisCmd.Parameters.AddRange(params.ToArray())
                    End If
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        thisAdapter.Fill(thisTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            thisTable = New DataTable()
        End Try
        Return thisTable
    End Function

    Public Sub ExecuteSP(spName As String, params As List(Of SqlParameter))
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(spName, thisConn)
                    thisCmd.CommandType = CommandType.StoredProcedure
                    If params IsNot Nothing AndAlso params.Count > 0 Then
                        thisCmd.Parameters.AddRange(params.ToArray())
                    End If
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetItemData_Integer(thisString As String) As Integer
        Dim result As Integer = 0
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Public Function GetItemData_Decimal(thisString As String) As Decimal
        Dim result As Double = 0D
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = 0D
        End Try
        Return result
    End Function

    Public Function GetItemData_Boolean(thisString As String) As Boolean
        Dim result As Boolean = False
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Function GetTotalDiscount(ParamArray discounts() As Object) As Decimal
        Try
            Dim result As Decimal = 1D

            If discounts Is Nothing OrElse discounts.Length = 0 Then
                Return 0D
            End If

            For Each d As Object In discounts
                Dim discValue As Decimal = 0D
                Try
                    If d Is Nothing Then
                        discValue = 0D
                    ElseIf TypeOf d Is Decimal OrElse TypeOf d Is Double OrElse TypeOf d Is Integer Then
                        discValue = Convert.ToDecimal(d)
                    ElseIf TypeOf d Is String Then
                        Dim s As String = d.ToString().Trim().ToUpper()
                        If s = "D" Then
                            discValue = 0D
                        ElseIf IsNumeric(s) Then
                            discValue = Convert.ToDecimal(s)
                        End If
                    End If
                Catch
                    discValue = 0D
                End Try
                result *= (1D - (discValue / 100D))
            Next

            Return (1D - result) * 100D
        Catch ex As Exception
            Return 0D
        End Try
    End Function

    Public Function GetTextLog(logId As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT '<b>' + Logins.FullName + '</b> on ' + FORMAT(Logs.ActionDate, 'dd MMM yyyy HH:mm') + '. Action : ' + Logs.Description AS FinalLog FROM Logs LEFT JOIN Logins ON Logs.ActionBy=Logins.Id WHERE Logs.Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", logId)
                    thisConn.Open()
                    Dim obj = thisCmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        result = obj.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Public Function GenerateNewPassword(length As Integer) As String
        Dim chars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
        Dim result As New StringBuilder()
        Dim crypto As New RNGCryptoServiceProvider()
        Dim data(length - 1) As Byte

        crypto.GetBytes(data)

        For Each b As Byte In data
            result.Append(chars(b Mod chars.Length))
        Next

        Return result.ToString()
    End Function

    Public Sub UpdateFailedCount(loginId As String)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE Logins SET FailedCount=0 WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", loginId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Function InsertSession() As String
        Dim result As String = String.Empty
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DECLARE @RawId NVARCHAR(50) = NEWID(); DECLARE @FinalId NVARCHAR(100) = 'RAP23-' + @RawId; INSERT INTO Sessions (Id, LoginId) VALUES (@FinalId, NULL); SELECT @FinalId;", thisConn)
                    thisConn.Open()
                    Dim newId As Object = thisCmd.ExecuteScalar()
                    If newId IsNot Nothing Then
                        result = newId.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return result
    End Function

    Public Sub UpdateSession(id As String, loginId As String)
        Dim ip As String = ""
        Dim browser As String = ""
        Dim device As String = ""
        Dim os As String = ""

        GetClientInfo(ip, browser, device, os)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE Sessions SET LoginId = @LoginId, IpAddress = @IpAddress, BrowserName = @BrowserName, Device = @Device, OS = @OS WHERE Id = @Id", thisConn)
                    thisCmd.Parameters.Add("@Id", SqlDbType.VarChar).Value = UCase(id)
                    thisCmd.Parameters.Add("@LoginId", SqlDbType.Int).Value = loginId
                    thisCmd.Parameters.Add("@IpAddress", SqlDbType.VarChar).Value = ip
                    thisCmd.Parameters.Add("@BrowserName", SqlDbType.VarChar).Value = browser
                    thisCmd.Parameters.Add("@Device", SqlDbType.VarChar).Value = device
                    thisCmd.Parameters.Add("@OS", SqlDbType.VarChar).Value = os
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub DeleteSession(sessId As String)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM Sessions WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", UCase(sessId).ToString())
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Private Sub GetClientInfo(ByRef ip As String, ByRef browser As String, ByRef device As String, ByRef os As String)
        ip = HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR")

        If String.IsNullOrWhiteSpace(ip) Then
            ip = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
        Else
            ip = ip.Split(","c)(0).Trim()
        End If

        Dim ua As String = If(HttpContext.Current.Request.UserAgent, "")

        If ua.Contains("Edg") Then
            browser = "Microsoft Edge"
        ElseIf ua.Contains("Chrome") AndAlso Not ua.Contains("Edg") Then
            browser = "Google Chrome"
        ElseIf ua.Contains("Firefox") Then
            browser = "Mozilla Firefox"
        ElseIf ua.Contains("Safari") AndAlso Not ua.Contains("Chrome") Then
            browser = "Safari"
        ElseIf ua.Contains("OPR") OrElse ua.Contains("Opera") Then
            browser = "Opera"
        Else
            browser = "Unknown"
        End If

        If ua.Contains("Android") OrElse ua.Contains("iPhone") OrElse ua.Contains("Mobile") Then
            device = "Mobile"
        ElseIf ua.Contains("iPad") OrElse ua.Contains("Tablet") Then
            device = "Tablet"
        Else
            device = "Desktop"
        End If

        If ua.Contains("Windows NT 10.0") Then
            os = "Windows 10/11"
        ElseIf ua.Contains("Windows") Then
            os = "Windows"
        ElseIf ua.Contains("Android") Then
            os = "Android"
        ElseIf ua.Contains("iPhone") Then
            os = "iOS"
        ElseIf ua.Contains("iPad") Then
            os = "iPadOS"
        ElseIf ua.Contains("Mac OS X") Then
            os = "macOS"
        ElseIf ua.Contains("Linux") Then
            os = "Linux"
        Else
            os = "Unknown"
        End If
    End Sub

    Public Function GenerateTicketId(length As Integer) As String
        Dim chars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
        Dim result As New StringBuilder()
        Dim crypto As New RNGCryptoServiceProvider()
        Dim data(length - 1) As Byte

        crypto.GetBytes(data)

        For Each b As Byte In data
            result.Append(chars(b Mod chars.Length))
        Next

        Return result.ToString()
    End Function

    Public Function GetProductAccess(companyId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                Dim hasil As String = String.Empty

                Dim cekDesign As DataTable = GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray WHERE companyArray.VALUE='" & companyId & "' ORDER BY Name ASC")
                If Not cekDesign.Rows.Count = 0 Then
                    For i As Integer = 0 To cekDesign.Rows.Count - 1
                        Dim id As String = cekDesign.Rows(i)("Id").ToString()
                        hasil += id.ToString() & ","
                    Next
                End If
                result = hasil.Remove(hasil.Length - 1).ToString()
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function Encrypt(clearText As String) As String
        Dim EncryptionKey As String = "BUM11ND4H9L084L"
        Dim clearBytes As Byte() = Encoding.Unicode.GetBytes(clearText)
        Using encryptor As Aes = Aes.Create()
            Dim pdb As New Rfc2898DeriveBytes(EncryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D, &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            encryptor.Key = pdb.GetBytes(32)
            encryptor.IV = pdb.GetBytes(16)
            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)
                    cs.Write(clearBytes, 0, clearBytes.Length)
                    cs.Close()
                End Using
                clearText = Convert.ToBase64String(ms.ToArray())
            End Using
        End Using
        Return clearText
    End Function

    Public Function Decrypt(cipherText As String) As String
        Dim EncryptionKey As String = "BUM11ND4H9L084L"
        Dim cipherBytes As Byte() = Convert.FromBase64String(cipherText)
        Using encryptor As Aes = Aes.Create()
            Dim pdb As New Rfc2898DeriveBytes(EncryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D, &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            encryptor.Key = pdb.GetBytes(32)
            encryptor.IV = pdb.GetBytes(16)
            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write)
                    cs.Write(cipherBytes, 0, cipherBytes.Length)
                    cs.Close()
                End Using
                cipherText = Encoding.Unicode.GetString(ms.ToArray())
            End Using
        End Using
        Return cipherText
    End Function

    Public Function GetRandomCode() As String
        Dim result As String = String.Empty
        Try
            Dim numbers As String = "1234567890"
            Dim characters As String = numbers

            Dim length As Integer = 6
            Dim otp As String = String.Empty
            For i As Integer = 0 To length - 1
                Dim character As String = String.Empty
                Do
                    Dim index As Integer = New Random().Next(0, characters.Length)
                    character = characters.ToCharArray()(index).ToString()
                Loop While otp.IndexOf(character) <> -1
                otp += character
            Next
            result = otp
        Catch ex As Exception
        End Try
        Return result
    End Function

    Public Function CustomerNewsletter(customerId As String) As Boolean
        Dim result As Boolean = False
        Try
            result = GetItemData_Boolean("SELECT Newsletter FROM Customers WHERE Id = '" + customerId + "'")
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Function CreateId(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Dim id As Integer = 0
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult As SqlDataReader = thisCmd.ExecuteReader()
                        If rdResult.Read() Then
                            Integer.TryParse(rdResult(0).ToString(), id)
                        End If
                    End Using
                End Using
            End Using
            result = (id + 1).ToString()
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GenerateUsername(accountName As String) As String
        Dim ignoreWords As String() = {"PT", "CV", "TOKO", "UD", "PD", "PTY", "LTD"}
        accountName = Regex.Replace(accountName, "[^a-zA-Z0-9 ]", "")
        Dim words As String() = accountName.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
        Dim filtered As New List(Of String)

        For Each w As String In words
            If Not ignoreWords.Contains(w.ToUpper()) Then
                filtered.Add(w.ToLower())
            End If
        Next
        If filtered.Count = 0 Then
            Return GetUniqueUsername("user")
        End If
        Dim candidates As New List(Of String)

        If filtered.Count >= 2 Then
            Dim w1 As String = filtered(0)
            Dim w2 As String = filtered(1)

            candidates.Add(Left(w1, Math.Min(4, w1.Length)) & Left(w2, Math.Min(4, w2.Length)))
            candidates.Add(Left(w1, Math.Min(5, w1.Length)) & Left(w2, Math.Min(3, w2.Length)))
            candidates.Add(Left(w1, Math.Min(3, w1.Length)) & Left(w2, Math.Min(5, w2.Length)))
            candidates.Add(w1 & w2)
            candidates.Add(Left(w1, 1) & w2)
            candidates.Add(Left(w1, 1) & Left(w2, 1))
        Else
            Dim w As String = filtered(0)
            candidates.Add(Left(w, Math.Min(8, w.Length)))
            candidates.Add(w)
        End If

        For Each candidate As String In candidates
            candidate = candidate.ToLower()
            If candidate.Length > 8 Then
                candidate = candidate.Substring(0, 8)
            End If
            If Not IsUsernameExists(candidate) Then
                Return candidate
            End If
        Next
        Return GetUniqueUsername(candidates(0))
    End Function

    Private Function GetUniqueUsername(baseUsername As String) As String
        baseUsername = baseUsername.ToLower()
        If baseUsername.Length > 8 Then
            baseUsername = baseUsername.Substring(0, 8)
        End If

        Dim username As String = baseUsername
        Dim counter As Integer = 1

        While IsUsernameExists(username)
            Dim suffix As String = counter.ToString()
            Dim maxLength As Integer = 8 - suffix.Length
            Dim prefix As String = baseUsername
            If prefix.Length > maxLength Then
                prefix = prefix.Substring(0, maxLength)
            End If
            username = prefix & suffix
            counter += 1
        End While
        Return username
    End Function

    Public Function IsUsernameExists(username As String) As Boolean
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using thisCmd As New SqlCommand("SELECT COUNT(1) FROM Logins WHERE LOWER(UserName) = @UserName", thisConn)
                thisCmd.Parameters.AddWithValue("@UserName", username.ToLower())

                Dim count As Integer = Convert.ToInt32(thisCmd.ExecuteScalar())
                Return count > 0
            End Using
        End Using
    End Function

    Public Sub Logs(data As Object())
        Try
            If data.Length = 4 Then
                Dim type As String = Convert.ToString(data(0))
                Dim dataId As String = Convert.ToString(data(1))
                Dim loginId As String = Convert.ToString(data(2))
                Dim description As String = Convert.ToString(data(3))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Logs VALUES (NEWID(), @Type, @DataId, @ActionBy, GETDATE(), @Description)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Type", type)
                        thisCmd.Parameters.AddWithValue("@DataId", If(String.IsNullOrEmpty(dataId), CType(DBNull.Value, Object), dataId))
                        thisCmd.Parameters.AddWithValue("@ActionBy", loginId)
                        thisCmd.Parameters.AddWithValue("@Description", description)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Function GetNewSortOrder(designId As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT TOP 1 SortOrder FROM Validations WHERE DesignId='" & designId & "' ORDER BY Id DESC", thisConn)
                    thisConn.Open()
                    Dim lastId As Object = thisCmd.ExecuteScalar()
                    If lastId IsNot Nothing AndAlso Not IsDBNull(lastId) Then
                        result = (CInt(lastId) + 1).ToString()
                    Else
                        result = "1"
                    End If
                End Using
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Shared ReadOnly ListValidationFieldName As List(Of String) =
        (New List(Of String) From {
            "headerid",
            "orderid",
            "itemaction",
            "itemid",
            "customerid",
            "companyid",
            "companydetailid",
            "loginid",
            "rolename",
            "designid",
            "blindtype",
            "tubetype",
            "controltype",
            "colourtype",
            "subtype",
            "qty",
            "qtyblade",
            "room",
            "mounting",
            "width",
            "widthb",
            "widthc",
            "widthd",
            "widthe",
            "widthf",
            "drop",
            "dropb",
            "dropc",
            "dropd",
            "drope",
            "dropf",
            "controlcolour",
            "controlcolourb",
            "stackposition",
            "stackpositionb",
            "roll",
            "rollb",
            "rollc",
            "rolld",
            "rolle",
            "rollf",
            "controlposition",
            "controlpositionb",
            "controlpositionc",
            "controlpositiond",
            "controlpositione",
            "controlpositionf",
            "tilterposition",
            "controllength",
            "controllengthvalue",
            "controllengthvalue2",
            "controllengthb",
            "controllengthvalueb",
            "controllengthvalueb2",
            "controllengthc",
            "controllengthvaluec",
            "controllengthvaluec2",
            "controllengthd",
            "controllengthvalued",
            "controllengthvalued2",
            "controllengthe",
            "controllengthvaluee",
            "controllengthvaluee2",
            "controllengthf",
            "controllengthvaluef",
            "controllengthvaluef2",
            "cordlengthvalue",
            "chainlengthvalue",
            "wandcolour",
            "wandlength",
            "wandlengthvalue",
            "wandlengthb",
            "wandlengthvalueb",
            "layoutcode",
            "layoutcodecustom",
            "tracktype",
            "tracktypeb",
            "trackcolour",
            "trackcolourb",
            "trackdraw",
            "trackdrawb",
            "toptrack",
            "supply",
            "tassel",
            "batten",
            "battenb",
            "valanceoption",
            "valancetype",
            "valancesize",
            "valancesizevalue",
            "returnposition",
            "returnlength",
            "returnlengthb",
            "returnlengthc",
            "returnlengthd",
            "returnlengthvalue",
            "returnlengthvalueb",
            "returnlengthvaluec",
            "returnlengthvalued",
            "fabrictype",
            "fabrictypeb",
            "fabrictypec",
            "fabrictyped",
            "fabrictypee",
            "fabrictypef",
            "fabriccolour",
            "fabriccolourb",
            "fabriccolourc",
            "fabriccolourd",
            "fabriccoloure",
            "fabriccolourf",
            "remote",
            "drycontact",
            "charger",
            "extensioncable",
            "chaincolour",
            "chaincolourb",
            "chaincolourc",
            "chaincolourd",
            "chaincoloure",
            "chaincolourf",
            "chainstopper",
            "chainstopperb",
            "chainstopperc",
            "chainstopperd",
            "chainstoppere",
            "chainstopperf",
            "bottomtype",
            "bottomtypeb",
            "bottomtypec",
            "bottomtyped",
            "bottomtypee",
            "bottomtypef",
            "bottomcolour",
            "bottomcolourb",
            "bottomcolourc",
            "bottomcolourd",
            "bottomcoloure",
            "bottomcolourf",
            "bottomoption",
            "bottomoptionb",
            "bottomoptionc",
            "bottomoptiond",
            "bottomoptione",
            "bottomoptionf",
            "springassist",
            "brackettype",
            "bracketsize",
            "bracketextension",
            "sloping",
            "isblindin",
            "fabricinsert",
            "bottomjoining",
            "heading",
            "headingb",
            "bottomhem",
            "tieback",
            "panelqty",
            "printing",
            "printingb",
            "printingc",
            "printingd",
            "printinge",
            "printingf",
            "printingg",
            "printingh",
            "adjusting",
            "cutlength",
            "louvresize",
            "louvreposition",
            "midrailheight1",
            "midrailheight2",
            "midrailcritical",
            "joinedpanels",
            "hingecolour",
            "semiinside",
            "customheaderlength",
            "framecolour",
            "frametype",
            "frameleft",
            "frameright",
            "frametop",
            "framebottom",
            "bottomtracktype",
            "bottomtrackrecess",
            "buildout",
            "buildoutposition",
            "samesizepanel",
            "gap1",
            "gap2",
            "gap3",
            "gap4",
            "gap5",
            "horizontaltpostheight",
            "horizontaltpost",
            "tiltrodtype",
            "tiltrodsplit",
            "splitheight1",
            "splitheight2",
            "reversehinged",
            "pelmetflat",
            "extrafascia",
            "hingesloose",
            "cutout",
            "specialshape",
            "templateprovided",
            "midrailposition",
            "handletype",
            "handlelength",
            "triplelock",
            "bugseal",
            "pettype",
            "petposition",
            "doorcloser",
            "beading",
            "jambtype",
            "jambposition",
            "flushbold",
            "interlocktype",
            "toptracklength",
            "bottomtrack",
            "bottomtracklength",
            "receivertype",
            "receiverlength",
            "slidingqty",
            "meshtype",
            "brace",
            "angletype",
            "anglelength",
            "angleqty",
            "porthole",
            "plungerpin",
            "swivelcolour",
            "swivelqty",
            "swivelqtyb",
            "springqty",
            "topplasticqty",
            "notes",
            "markup",
            "sellprice",
            "buyprice",
            "pricegroupid",
            "total"
        }).Select(Function(x) x.ToUpperInvariant()).ToList()

    Public Shared ReadOnly ListValidationOperator As New List(Of String) From {
        "NOT NULL", "NULL", "=", "<>", "CONTAINS", "NOT CONTAINS", "STARTS WITH", "ENDS WITH", "IN", "NOT IN", "IS_NUMBER", "POSITIVE_INTEGER", "POSITIVE_DECIMAL", "INVALID_CHARS", "EQUAL_FIELD", "NOT_EQUAL_FIELD", ">", ">=", "<", "<="
    }
End Class