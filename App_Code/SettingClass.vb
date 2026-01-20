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
                        Dim dt As New DataTable()
                        thisAdapter.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Return dt.Rows(0)
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

    Public Function GetDataTable(thisString As String) As DataTable
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using da As New SqlDataAdapter(thisCmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return dt
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
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
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
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
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
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
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
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
        Dim result As Decimal = 1D

        For Each d As Object In discounts
            Dim discValue As Decimal = 0D

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

            result *= (1D - (discValue / 100D))
        Next

        Return (1D - result) * 100D
    End Function

    Public Function GetTextLog(logId As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("SELECT '<b>' + CustomerLogins.FullName + '</b> on ' + FORMAT(Logs.ActionDate, 'dd MMM yyyy HH:mm') + '. Action : ' + Logs.Description AS FinalLog FROM Logs LEFT JOIN CustomerLogins ON Logs.ActionBy = CustomerLogins.Id  WHERE Logs.Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", logId)

                    thisConn.Open()
                    Dim obj = myCmd.ExecuteScalar()
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

    Public Function InsertSession() As String
        Dim result As String = String.Empty
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DECLARE @RawId NVARCHAR(50) = NEWID(); DECLARE @FinalId NVARCHAR(100) = 'RAP23-' + @RawId; INSERT INTO Sessions (Id, LoginId) VALUES (@FinalId, NULL); SELECT @FinalId;", thisConn)
                    thisConn.Open()

                    Dim newId As Object = myCmd.ExecuteScalar()
                    If newId IsNot Nothing Then
                        result = newId.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return result
    End Function

    Public Sub UpdateFailedCount(loginId As String)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET FailedCount=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", loginId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub UpdateSession(id As String, loginId As String)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Sessions SET LoginId=@LoginId WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", UCase(id).ToString())
                    myCmd.Parameters.AddWithValue("@LoginId", loginId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub DeleteSession(sessId As String)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Sessions WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", UCase(sessId).ToString())

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
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
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult As SqlDataReader = myCmd.ExecuteReader()
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

    Public Sub Logs(data As Object())
        Try
            If data.Length = 4 Then
                Dim type As String = Convert.ToString(data(0))
                Dim dataId As String = Convert.ToString(data(1))
                Dim loginId As String = Convert.ToString(data(2))
                Dim description As String = Convert.ToString(data(3))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Logs VALUES (NEWID(), @Type, @DataId, @ActionBy, GETDATE(), @Description)", thisConn)
                        myCmd.Parameters.AddWithValue("@Type", type)
                        myCmd.Parameters.AddWithValue("@DataId", If(String.IsNullOrEmpty(dataId), CType(DBNull.Value, Object), dataId))
                        myCmd.Parameters.AddWithValue("@ActionBy", loginId)
                        myCmd.Parameters.AddWithValue("@Description", description)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
