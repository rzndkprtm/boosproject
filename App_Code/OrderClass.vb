Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Public Class OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")

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

    Public Function IsValidEmail(email As String) As Boolean
        Try
            Dim mail As New Net.Mail.MailAddress(email)
            Return True
        Catch
            Return False
        End Try
    End Function

    Public Function GetOrderContext(id As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(id) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT QueryString FROM OrderActionContext WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", id)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetOrderType(headerId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(headerId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT OrderType FROM OrderHeaders WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", headerId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetCustomerPrimaryEmail(customerId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(customerId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Email FROM CustomerContacts WHERE CustomerId=@CustomerId AND [Primary]=1", thisConn)
                        thisCmd.Parameters.AddWithValue("@CustomerId", customerId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetUserRoleName(loginId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(loginId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT LoginRoles.Name FROM Logins INNER JOIN LoginRoles ON Logins.RoleId=LoginRoles.Id WHERE Logins.Id=@LoginId", thisConn)
                        thisCmd.Parameters.AddWithValue("@LoginId", loginId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetCustomerPriceAccess(loginId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(loginId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT CASE WHEN Pricing=1 THEN 'Yes' ELSE '' END AS PriceAccess FROM Logins WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", loginId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetCustomerIdByOrder(headerId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(headerId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT CustomerId FROM OrderHeaders WHERE Id=@OrderId", thisConn)
                        thisCmd.Parameters.AddWithValue("@OrderId", headerId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetCompanyIdByOrder(orderId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(orderId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Customers.CompanyId FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id=@OrderId", thisConn)
                        thisCmd.Parameters.AddWithValue("@OrderId", orderId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetCompanyDetailIdByOrder(orderId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(orderId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Customers.CompanyDetailId FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id=@OrderId", thisConn)
                        thisCmd.Parameters.AddWithValue("@OrderId", orderId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetCompanyAliasByCustomer(customerId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(customerId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Companys.Alias FROM Customers LEFT JOIN Companys ON Customers.CompanyId=Companys.Id WHERE Customers.Id=@CustomerId", thisConn)
                        thisCmd.Parameters.AddWithValue("@CustomerId", customerId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetCompanyDetailName(companyDetailId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(companyDetailId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM CompanyDetails WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", companyDetailId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetCompanyDetailIdByCustomer(customerId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(customerId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT CompanyDetailId FROM Customers WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", customerId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetCompanyDetailNameByCustomer(customerId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(customerId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT CompanyDetails.Name FROM Customers LEFT JOIN CompanyDetails ON Customers.CompanyDetailId=CompanyDetails.Id WHERE Customers.Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", customerId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetPriceGroupByOrder(orderId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(orderId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Customers.PriceGroupId FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id=@OrderId", thisConn)
                        thisCmd.Parameters.AddWithValue("@OrderId", orderId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetPriceProductGroupId(groupName As String, designId As String, companyDetailId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(groupName) OrElse String.IsNullOrEmpty(designId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Id FROM PriceProductGroups CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS thisArray WHERE Name=@Name AND DesignId=@DesignId AND thisArray.VALUE=@CompanyDetailId AND Active=1", thisConn)
                        thisCmd.Parameters.AddWithValue("@Name", groupName)
                        thisCmd.Parameters.AddWithValue("@DesignId", designId)
                        thisCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetailId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetPriceProductGroupName(groupId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(groupId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM PriceProductGroups WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", groupId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetDesignId(productId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(productId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT DesignId FROM Products WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", productId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetBlindId(productId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(productId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT BlindId FROM Products WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", productId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetTubeId(productId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(productId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT TubeType FROM Products WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", productId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetControlId(productId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(productId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT ControlType FROM Products WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", productId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetDesignName(designId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(designId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM Designs WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", designId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetDesignPage(designId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(designId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Page FROM Designs WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", designId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetBlindName(blindId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(blindId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM Blinds WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", blindId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetProductName(productId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(productId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM Products WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", productId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetProductDescription(itemId As String) As String
        Dim result As String = String.Empty
        Try
            Dim param As New List(Of SqlParameter) From {
                New SqlParameter("@ItemId", Convert.ToInt32(itemId))
            }
            Dim thisData As DataRow = GetDataRowSP("sp_OrderDetails_Description", param)
            If thisData Is Nothing Then
                Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
            End If

            Dim productId As String = thisData("ProductId").ToString()
            Dim productName As String = thisData("ProductName").ToString()
            Dim designName As String = thisData("DesignName").ToString()
            Dim blindName As String = thisData("BlindName").ToString()

            Dim subType As String = thisData("SubType").ToString()
            Dim heading As String = thisData("Heading").ToString()
            Dim headingB As String = thisData("HeadingB").ToString()

            Dim trackType As String = thisData("TrackType").ToString()
            Dim trackTypeB As String = thisData("TrackTypeB").ToString()

            Dim fabricColourId As String = thisData("FabricColourId").ToString()
            Dim fabricColourIdB As String = thisData("FabricColourIdB").ToString()
            Dim fabricColourIdC As String = thisData("FabricColourIdC").ToString()
            Dim fabricColourIdD As String = thisData("FabricColourIdD").ToString()
            Dim fabricColourIdE As String = thisData("FabricColourIdE").ToString()
            Dim fabricColourIdF As String = thisData("FabricColourIdF").ToString()

            Dim width As String = thisData("Width").ToString()
            Dim widthB As String = thisData("WidthB").ToString()
            Dim widthC As String = thisData("WidthC").ToString()
            Dim widthD As String = thisData("WidthD").ToString()
            Dim widthE As String = thisData("WidthE").ToString()
            Dim widthF As String = thisData("WidthF").ToString()

            Dim drop As String = thisData("Drop").ToString()
            Dim dropB As String = thisData("DropB").ToString()
            Dim dropC As String = thisData("DropC").ToString()
            Dim dropD As String = thisData("DropD").ToString()
            Dim dropE As String = thisData("DropE").ToString()
            Dim dropF As String = thisData("DropF").ToString()

            Dim printing As String = thisData("Printing").ToString()
            Dim printingB As String = thisData("PrintingB").ToString()
            Dim printingC As String = thisData("PrintingC").ToString()
            Dim printingD As String = thisData("PrintingD").ToString()
            Dim printingE As String = thisData("PrintingE").ToString()
            Dim printingF As String = thisData("PrintingF").ToString()

            Dim layoutCode As String = thisData("LayoutCode").ToString()
            Dim frameColour As String = thisData("FrameColour").ToString()

            Dim itemNote As String = thisData("Notes").ToString()

            Dim size As String = String.Format("({0}x{1})", width, drop)
            Dim sizeB As String = String.Format("({0}x{1})", widthB, dropB)
            Dim sizeC As String = String.Format("({0}x{1})", widthC, dropC)
            Dim sizeD As String = String.Format("({0}x{1})", widthD, dropD)
            Dim sizeE As String = String.Format("({0}x{1})", widthE, dropE)
            Dim sizeF As String = String.Format("({0}x{1})", widthF, dropF)

            Dim fabricColourName As String = GetFabricColourName(fabricColourId)
            Dim fabricColourNameB As String = GetFabricColourName(fabricColourIdB)
            Dim fabricColourNameC As String = GetFabricColourName(fabricColourIdC)
            Dim fabricColourNameD As String = GetFabricColourName(fabricColourIdD)
            Dim fabricColourNameE As String = GetFabricColourName(fabricColourIdE)
            Dim fabricColourNameF As String = GetFabricColourName(fabricColourIdF)

            Dim squareMetre As Decimal = 0D
            Dim squareMetreB As Decimal = 0D
            Dim squareMetreC As Decimal = 0D
            Dim squareMetreD As Decimal = 0D
            Dim squareMetreE As Decimal = 0D
            Dim squareMetreF As Decimal = 0D

            If Not IsDBNull(thisData("SquareMetre")) Then
                squareMetre = Math.Round(Convert.ToDecimal(thisData("SquareMetre")), 2)
            End If
            If Not IsDBNull(thisData("SquareMetreB")) Then
                squareMetreB = Math.Round(Convert.ToDecimal(thisData("SquareMetreB")), 2)
            End If
            If Not IsDBNull(thisData("SquareMetreC")) Then
                squareMetreC = Math.Round(Convert.ToDecimal(thisData("SquareMetreC")), 2)
            End If
            If Not IsDBNull(thisData("SquareMetreD")) Then
                squareMetreD = Math.Round(Convert.ToDecimal(thisData("SquareMetreD")), 2)
            End If
            If Not IsDBNull(thisData("SquareMetreE")) Then
                squareMetreE = Math.Round(Convert.ToDecimal(thisData("SquareMetreE")), 2)
            End If
            If Not IsDBNull(thisData("SquareMetreF")) Then
                squareMetreF = Math.Round(Convert.ToDecimal(thisData("SquareMetreF")), 2)
            End If

            Dim squareMetreText As String = String.Format("{0}sqm", squareMetre.ToString("0.##", enUS))
            Dim squareMetreTextB As String = String.Format("{0}sqm", squareMetreB.ToString("0.##", enUS))
            Dim squareMetreTextC As String = String.Format("{0}sqm", squareMetreC.ToString("0.##", enUS))
            Dim squareMetreTextD As String = String.Format("{0}sqm", squareMetreD.ToString("0.##", enUS))
            Dim squareMetreTextE As String = String.Format("{0}sqm", squareMetreE.ToString("0.##", enUS))
            Dim squareMetreTextF As String = String.Format("{0}sqm", squareMetreF.ToString("0.##", enUS))

            Dim linearMetre As Decimal = 0D
            Dim linearMetreB As Decimal = 0D
            Dim linearMetreC As Decimal = 0D

            If Not IsDBNull(thisData("LinearMetre")) Then
                linearMetre = Math.Round(Convert.ToDecimal(thisData("LinearMetre")), 2)
            End If
            If Not IsDBNull(thisData("LinearMetreB")) Then
                linearMetreB = Math.Round(Convert.ToDecimal(thisData("LinearMetreB")), 2)
            End If
            If Not IsDBNull(thisData("LinearMetreC")) Then
                linearMetreC = Math.Round(Convert.ToDecimal(thisData("LinearMetreC")), 2)
            End If

            Dim linearMetreText As String = String.Format("{0}lm", linearMetre.ToString("0.##", enUS))
            Dim linearMetreTextB As String = String.Format("{0}lm", linearMetreB.ToString("0.##", enUS))
            Dim linearMetreTextC As String = String.Format("{0}lm", linearMetreC.ToString("0.##", enUS))

            Dim room As String = thisData("Room").ToString()
            Dim itemDescription As String = String.Format("<b>{0}</b>, {1}", room, productName)

            If designName = "Aluminium Blind" Then
                result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
                If subType.Contains("2 on 1") Then
                    result = String.Format("<b>{0}</b>, 2 on 1 Headrail", room)
                    result &= "<br />"
                    result &= String.Format("{0} {1} {2}", productName, size, squareMetreText)
                    result &= "<br />"
                    result &= String.Format("{0} {1} {2}", productName, sizeB, squareMetreTextB)
                End If
            End If

            If designName = "Cellular Shades" Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
                If blindName = "Day & Night" Then
                    result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
                    result &= "<br />"
                    result &= fabricColourName
                    result &= "<br />"
                    result &= fabricColourNameB
                End If
            End If

            If designName = "Curtain" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("{0} {1} {2} | {3} ({4}) {5}", fabricColourName, size, squareMetreText, trackType, width, linearMetreText)

                If blindName = "Complete Set (Double)" Then
                    result = itemDescription
                    result &= "<br />"
                    result &= String.Format("{0} {1} {2} | {3} ({4}) {5}", fabricColourName, size, squareMetreText, trackType, width, linearMetreText)
                    result &= "<br />"
                    result &= String.Format("{0} {1} {2} | {3} ({4}) {5}", fabricColourNameB, sizeB, squareMetreTextB, trackTypeB, widthB, linearMetreTextB)
                End If
                If blindName = "Curtain Only" Then
                    result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
                End If
                If blindName = "Fabric Only" Then
                    result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
                End If
                If blindName = "Track Only" Then
                    result = String.Format("{0} {1} ({2}) {3}", itemDescription, trackType, width, linearMetreText)
                End If
            End If

            If designName = "Design Shades" Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
            End If

            If designName = "Linea Valance" Then
                result = String.Format("{0} ({1}mm) {2}", itemDescription, width, linearMetreText)
            End If

            If designName = "Panel Glide" Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
                If blindName = "Track Only" Then
                    result = String.Format("{0} ({1}mm) {2}", itemDescription, width, linearMetreText)
                End If
            End If

            If designName = "Pelmet" Then
                result = String.Format("{0} {1} ({2}mm) {3}", itemDescription, fabricColourName, width, linearMetreText)
                If layoutCode = "B" OrElse layoutCode = "C" Then
                    result = itemDescription
                    result &= "<br />"
                    result &= String.Format("{0} ({1}mm) {2}", fabricColourName, width, linearMetreText)
                    result &= "<br />"
                    result &= String.Format("{0} ({1}mm) {2}", fabricColourName, widthB, linearMetreTextB)
                End If

                If layoutCode = "D" Then
                    result = itemDescription
                    result &= "<br />"
                    result &= String.Format("{0} ({1}mm) {2}", fabricColourName, width, linearMetreText)
                    result &= "<br />"
                    result &= String.Format("{0} ({1}mm) {2}", fabricColourName, widthB, linearMetreTextB)
                    result &= "<br />"
                    result &= String.Format("{0} ({1}mm) {2}", fabricColourName, widthC, linearMetreTextC)
                End If
            End If

            If designName = "Privacy Venetian" Then
                result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
            End If

            If designName = "Roller Blind" Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
                If Not String.IsNullOrEmpty(printing) Then
                    result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
                    result &= "<br />"
                    result &= "<b>[Printed Fabric]</b>"
                End If
                If blindName = "Dual Blinds" Then
                    result = itemDescription
                    result &= "<br />"
                    result &= String.Format("First Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                    If Not String.IsNullOrEmpty(printing) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                    If Not String.IsNullOrEmpty(printingB) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                End If
                If blindName = "Link 2 Blinds Dependent" Then
                    result = itemDescription
                    result &= "<br />"
                    result &= String.Format("First & Control Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                    If Not String.IsNullOrEmpty(printing) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                    If Not String.IsNullOrEmpty(printingB) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                End If
                If blindName = "Link 2 Blinds Independent" Then
                    result = itemDescription
                    result &= "<br />"
                    result &= String.Format("Left Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                    If Not String.IsNullOrEmpty(printing) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Right Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                    If Not String.IsNullOrEmpty(printingB) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                End If
                If blindName = "Link 3 Blinds Dependent" Then
                    result = itemDescription
                    result &= "<br />"
                    result &= String.Format("Control Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                    If Not String.IsNullOrEmpty(printing) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                    If Not String.IsNullOrEmpty(printingB) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Third Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                    If Not String.IsNullOrEmpty(printingC) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                End If

                If blindName = "Link 3 Blinds Independent with Dependent" Then
                    result = itemDescription
                    result &= "<br />"
                    result &= String.Format("Independent Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                    If Not String.IsNullOrEmpty(printing) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                    If Not String.IsNullOrEmpty(printingB) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Control Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                    If Not String.IsNullOrEmpty(printingC) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                End If

                If blindName = "Link 4 Blinds Independent with Dependent" Then
                    result = itemDescription
                    result &= "<br />"
                    result &= String.Format("Left Control Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                    If Not String.IsNullOrEmpty(printing) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Middle Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                    If Not String.IsNullOrEmpty(printingB) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Middle Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                    If Not String.IsNullOrEmpty(printingC) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Right Control Blind : {0} {1} {2}", fabricColourNameD, sizeD, squareMetreTextD)
                    If Not String.IsNullOrEmpty(printingD) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                End If

                If blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 2 Blinds Independent" Then
                    result = itemDescription
                    result &= "<br />"
                    result &= String.Format("First Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                    If Not String.IsNullOrEmpty(printing) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                    If Not String.IsNullOrEmpty(printingB) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Third Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                    If Not String.IsNullOrEmpty(printingC) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Fourth Blind : {0} {1} {2}", fabricColourNameD, sizeD, squareMetreTextD)
                    If Not String.IsNullOrEmpty(printingD) Then
                        result &= " (<b>[Printed Fabric]</b>>)"
                    End If
                End If

                If blindName = "DB Link 3 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" Then
                    result = itemDescription
                    result &= "<br />"
                    result &= String.Format("First Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                    If Not String.IsNullOrEmpty(printing) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                    If Not String.IsNullOrEmpty(printingB) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Third Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                    If Not String.IsNullOrEmpty(printingC) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Fourth Blind : {0} {1} {2}", fabricColourNameD, sizeD, squareMetreTextD)
                    If Not String.IsNullOrEmpty(printingD) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Fifth Blind : {0} {1} {2}", fabricColourNameE, sizeE, squareMetreTextE)
                    If Not String.IsNullOrEmpty(printingE) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                    result &= "<br />"
                    result &= String.Format("Sixth Blind : {0} {1} {2}", fabricColourNameF, sizeF, squareMetreTextF)
                    If Not String.IsNullOrEmpty(printingF) Then
                        result &= " (<b>[Printed Fabric]</b>)"
                    End If
                End If
            End If

            If designName = "Roman Blind" Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
                If Not String.IsNullOrEmpty(printing) Then
                    result &= "<br />"
                    result &= "<b>[Printed Fabric]</b>"
                End If
            End If

            If designName = "Soft Roman" Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
                If Not String.IsNullOrEmpty(printing) Then
                    result &= "<br />"
                    result &= "<b>[Printed Fabric]</b>"
                End If
            End If

            If designName = "Sample" Then
                result = productName
            End If

            If designName = "Skyline Shutter Express" Then
                result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
            End If

            If designName = "Skyline Shutter Ocean" Then
                result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
            End If

            If designName = "Evolve Shutter Ocean" Then
                result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
            End If

            If designName = "Venetian Blind" Then
                result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
                If subType.Contains("2 on 1") Then
                    result = String.Format("<b>{0}</b>, 2 on 1 Headrail", room)
                    result &= "<br />"
                    result &= String.Format("{0} {1} {2}", productName, size, squareMetreText)
                    result &= "<br />"
                    result &= String.Format("{0} {1} {2}", productName, sizeB, squareMetreTextB)
                End If
                If subType.Contains("3 on 1") Then
                    result = String.Format("<b>{0}</b>, 3 on 1 Headrail", room)
                    result &= "<br />"
                    result &= String.Format("{0} {1} {2}", productName, size, squareMetreText)
                    result &= "<br />"
                    result &= String.Format("{0} {1} {2}", productName, sizeB, squareMetreTextB)
                    result &= "<br />"
                    result &= String.Format("{0} {1} {2}", productName, sizeC, squareMetreTextC)
                End If
            End If

            If designName = "Vertical" Then
                fabricColourName = fabricColourName.Replace("127mm ", "").Replace("89mm ", "").Trim()
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
                If blindName = "Track Only" Then
                    result = String.Format("{0} ({1}mm) {2}", itemDescription, width, linearMetreText)
                End If
            End If

            If designName = "Saphora Drape" Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
            End If

            If designName = "Window" OrElse designName = "Door" Then
                result = String.Format("{0} - {1} {2} {3}", itemDescription, frameColour, size, squareMetreText)
            End If

            If designName = "Outdoor" Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
            End If

            If designName = "Roller Horizon" Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
            End If

            If designName = "Service" Then
                result = productName
            End If

            Dim checkNote As String = GetItemData("SELECT Description FROM OrderCostings WHERE ItemId='" & itemId & "' AND Type='Note'")
            If Not String.IsNullOrEmpty(checkNote) Then
                result &= "<br />"
                result &= String.Format("<i>* {0} </i>", checkNote)
            End If

            Return result
        Catch ex As Exception
            Return result
        End Try
    End Function

    Public Function GetTubeName(tubeId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(tubeId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM ProductTubes WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", tubeId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function    

    Public Function GetControlType(controlId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(controlId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Type FROM ProductControls WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", controlId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetControlName(controlId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(controlId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM ProductControls WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", controlId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetColourName(colourId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(colourId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM ProductColours WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", colourId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetFabricName(fabricId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(fabricId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM Fabrics WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", fabricId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetFabricColourName(fabricColourId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(fabricColourId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM FabricColours WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", fabricColourId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetFabricGroup(fabricId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(fabricId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT [Group] FROM Fabrics WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", fabricId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetFabricGroupLocal(product As String, fabricId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(product) AndAlso Not String.IsNullOrEmpty(fabricId) Then
                Dim thisQuery As String = String.Format("SELECT {0} FROM FabricGroupLocals WHERE Id=@Id", product)

                If Not String.IsNullOrEmpty(thisQuery) Then
                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As New SqlCommand(thisQuery, thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", fabricId)
                            thisConn.Open()
                            Dim obj = thisCmd.ExecuteScalar()
                            If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                                result = obj.ToString()
                            End If
                        End Using
                    End Using
                End If
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetFabricFactory(fabricColourId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(fabricColourId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Factory FROM FabricColours WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", fabricColourId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetChainName(chainId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(chainId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM Chains WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", chainId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetChainType(chainId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(chainId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT ChainType FROM Chains WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", chainId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetChainLength(chainId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(chainId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT ChainLength FROM Chains WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", chainId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetBottomName(bottomId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(bottomId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM Bottoms WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", bottomId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GenerateRandomCode() As String
        Try
            Dim chars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
            Dim rnd As New Random()
            Dim result As New StringBuilder()
            For i As Integer = 1 To 5
                Dim idx As Integer = rnd.Next(0, chars.Length)
                result.Append(chars(idx))
            Next
            Return result.ToString()
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    Public Function GetNewOrderHeaderId() As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderHeaders ORDER BY Id DESC", thisConn)
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

    Public Function GetNewOrderItemId() As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderDetails ORDER BY Id DESC", thisConn)
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

    Public Function GetNewOrderReworkId() As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderReworks ORDER BY Id DESC", thisConn)
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

    Public Function GetNewOrderReworkDetailId() As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderReworkDetails ORDER BY Id DESC", thisConn)
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

    Public Function CreateOrderJobId() As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderJobs ORDER BY Id DESC", thisConn)
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

    Public Function CreateItemNumberJob(orderJobId As String, jobSheetId As String) As Integer
        Dim result As Integer = 0
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT TOP 1 ItemNumber FROM OrderJobDetails WHERE OrderJobId='" & orderJobId & "' AND JobSheetId='" & jobSheetId & "' ORDER BY ItemNumber DESC", thisConn)
                    thisConn.Open()
                    Dim lastId As Object = thisCmd.ExecuteScalar()
                    If lastId IsNot Nothing AndAlso Not IsDBNull(lastId) Then
                        result = CInt(lastId) + 1
                    Else
                        result = "1"
                    End If
                End Using
            End Using
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Public Function IsOrderExist(customerId As String, orderNumber As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT OrderNumber FROM OrderHeaders WHERE CustomerId=@CustomerId AND OrderNumber=@OrderNumber AND Active=1", thisConn)
                    thisCmd.Parameters.AddWithValue("@CustomerId", customerId)
                    thisCmd.Parameters.AddWithValue("@OrderNumber", orderNumber)
                    thisConn.Open()
                    Dim obj = thisCmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        result = obj.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetCustomerCashSale(customerId As String) As Boolean
        Dim result As Boolean = False
        Try
            If Not String.IsNullOrEmpty(customerId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT CashSale FROM Customers WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", customerId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Function GetCustomerOnStop(customerId As String) As Boolean
        Dim result As Boolean = False
        Try
            If Not String.IsNullOrEmpty(customerId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT OnStop FROM Customers WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", customerId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Function GetCustomerMinimum(customerId As String) As Boolean
        Dim result As Boolean = True
        Try
            If Not String.IsNullOrEmpty(customerId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT MinSurcharge FROM Customers WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", customerId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = True
        End Try
        Return result
    End Function

    Public Function GetTotalItemOrder(headerId As String) As Integer
        Dim result As String = 0
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT SUM (CASE WHEN Designs.Type='Blinds' THEN OrderDetails.TotalItems WHEN Designs.Type='Shutters' OR Designs.Type = 'Doors' THEN 1 ELSE 0 END) AS totalOrder FROM OrderHeaders INNER JOIN OrderDetails ON OrderHeaders.Id=OrderDetails.HeaderId INNER JOIN Products ON OrderDetails.ProductId=Products.Id INNER JOIN Designs ON Products.DesignId=Designs.Id WHERE OrderHeaders.Id=@Id AND OrderHeaders.Active=1 AND OrderDetails.Active=1 AND Designs.Type IN ('Blinds', 'Shutters', 'Doors', 'Samples');", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", headerId)
                    thisConn.Open()
                    Dim obj = thisCmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        result = obj.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Public Function GetTextLog(logId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(logId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT '<b>' + Logins.FullName + '</b> on ' + FORMAT(Logs.ActionDate, 'dd MMMM yyyy HH:mm') + '. Action : ' + Logs.Description AS FinalLog FROM Logs LEFT JOIN Logins ON Logs.ActionBy = Logins.Id WHERE Logs.Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", logId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Public Sub UpdateOrderFactory(headerId As String)
        Try
            Try
                Dim factoryList As New List(Of String)

                Dim detailData As DataTable = GetDataTable("SELECT OrderDetails.*, Products.Name AS ProductName, Designs.Name AS DesignName, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.DesignId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1")
                For iDetail As Integer = 0 To detailData.Rows.Count - 1
                    Dim designName As String = detailData.Rows(iDetail)("DesignName").ToString()
                    Dim blindName As String = detailData.Rows(iDetail)("BlindName").ToString()

                    Dim isBig As Boolean = False
                    Dim isAustralia As Boolean = False
                    Dim isTaiwan As Boolean = False
                    Dim isChina As Boolean = False

                    If designName = "Aluminium Blind" OrElse designName = "Design Shades" OrElse designName = "Linea Valance" OrElse designName = "Panel Glide" OrElse designName = "Pelmet" OrElse designName = "Roman Blind" OrElse designName = "Soft Roman" OrElse designName = "Privacy Venetian" OrElse designName = "Venetian Blind" OrElse designName = "Vertical" OrElse designName = "Roller Blind" OrElse designName = "Sample" OrElse designName = "Skyline Shutter Express" OrElse designName = "Outdoor" OrElse designName = "Saphora Drape" OrElse designName = "Roller Horizon" Then
                        isBig = True
                    End If

                    If designName = "Cellular Shades" Then
                        Dim fabricColourId As String = detailData.Rows(iDetail)("FabricColourId").ToString()
                        Dim fabricFactory As String = GetItemData("SELECT Factory FROM FabricColours WHERE Id='" & fabricColourId & "'")

                        If fabricFactory = "Express" Then isBig = True
                        If fabricFactory = "Regular" Then isTaiwan = True
                    End If

                    If designName = "Curtain" Then
                        Dim fabricColourId As String = detailData.Rows(iDetail)("FabricColourId").ToString()
                        Dim fabricColourIdB As String = detailData.Rows(iDetail)("FabricColourIdB").ToString()

                        Dim trackType As String = detailData.Rows(iDetail)("TrackType").ToString()
                        Dim trackTypeB As String = detailData.Rows(iDetail)("TrackTypeB").ToString()

                        Dim fabricFactory As String = String.Empty
                        Dim fabricFactoryB As String = String.Empty

                        If fabricColourId <> "" Then
                            fabricFactory = GetItemData("SELECT Factory FROM FabricColours WHERE Id='" & fabricColourId & "'")
                        End If

                        If fabricColourIdB <> "" Then
                            fabricFactoryB = GetItemData("SELECT Factory FROM FabricColours WHERE Id='" & fabricColourIdB & "'")
                        End If

                        If fabricFactory = "Express" Then
                            isBig = True
                        ElseIf fabricFactory = "Regular" Then
                            isAustralia = True
                        End If

                        If fabricFactoryB = "Express" Then
                            isBig = True
                        ElseIf fabricFactoryB = "Regular" Then
                            isAustralia = True
                        End If

                        If trackType = "Standard Track" Then
                            isAustralia = True
                        ElseIf trackType = "Motorised Track" Then
                            isBig = True
                        End If

                        If trackTypeB = "Standard Track" Then
                            isAustralia = True
                        ElseIf trackTypeB = "Motorised Track" Then
                            isBig = True
                        End If
                    End If

                    If designName = "Skyline Shutter Ocean" OrElse designName = "Evolve Shutter Ocean" Then
                        isChina = True
                    End If

                    If designName = "Door" OrElse designName = "Window" Then
                        Dim frameColour As String = detailData.Rows(iDetail)("FrameColour").ToString()

                        If frameColour.Contains("Regular") Then isAustralia = True
                        If frameColour.Contains("Express") Then isBig = True
                    End If

                    If isBig AndAlso Not factoryList.Contains("BIG") Then
                        factoryList.Add("BIG")
                    End If

                    If isTaiwan AndAlso Not factoryList.Contains("TAIWAN") Then
                        factoryList.Add("TAIWAN")
                    End If

                    If isAustralia AndAlso Not factoryList.Contains("AUS") Then
                        factoryList.Add("AUS")
                    End If

                    If isChina AndAlso Not factoryList.Contains("CHINA") Then
                        factoryList.Add("CHINA")
                    End If
                Next

                Dim factory As String = String.Join(", ", factoryList)

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET OrderFactory=@OrderFactory WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", headerId)
                        thisCmd.Parameters.AddWithValue("@OrderFactory", factory)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using
            Catch ex As Exception
            End Try
        Catch ex As Exception

        End Try
    End Sub


    ' PRICING

    Public Function GetGridPrice(data As Object()) As DataRow
        Try
            Dim productGroupId As String = Convert.ToString(data(0))
            Dim priceGroupId As String = Convert.ToString(data(1))
            Dim drop As Integer = Convert.ToInt32(data(2))
            Dim width As Integer = Convert.ToInt32(data(3))
            Dim category As String = Convert.ToString(data(4))

            If priceGroupId = "5" OrElse priceGroupId = "19" Then
                If drop > 0 AndAlso drop < 1000 Then drop = 1000
                If width > 0 AndAlso width < 1000 Then width = 1000
            End If

            Dim thisString As String = String.Format("SELECT TOP 1 * FROM PriceBases WHERE ProductGroupId='{0}' AND PriceGroupId='{1}' AND Height>='{2}' AND Width>='{3}' AND Category='{4}' AND Price>=0 ORDER BY Height, Width, Price, Conditional ASC", productGroupId, priceGroupId, drop, width, category)

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

    Public Sub ResetPriceDetail(headerId As String, itemId As String)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM OrderCostings WHERE HeaderId=@HeaderId AND ItemId=@ItemId", thisConn)
                    thisCmd.Parameters.AddWithValue("@HeaderId", headerId)
                    thisCmd.Parameters.AddWithValue("@ItemId", itemId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub UpdateServiceItem(headerId As String, itemId As String, buyPrice As Decimal, sellPrice As Decimal)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderCostings SET BuyPrice=@BuyPrice, SellPrice=@SellPrice WHERE HeaderId=@HeaderId AND ItemId=@ItemId AND Type='Base'", thisConn)
                    thisCmd.Parameters.AddWithValue("@HeaderId", headerId)
                    thisCmd.Parameters.AddWithValue("@ItemId", itemId)
                    thisCmd.Parameters.AddWithValue("@BuyPrice", buyPrice)
                    thisCmd.Parameters.AddWithValue("@SellPrice", sellPrice)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub FinalCostItem(headerId As String, itemId As String)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM OrderCostings WHERE ItemId=@ItemId AND Type='Final'", thisConn)
                    thisCmd.Parameters.AddWithValue("@ItemId", itemId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim sellPrice As Decimal = GetItemData_Decimal("SELECT SUM(CASE WHEN Type='Base' THEN SellPrice WHEN Type='Discount' THEN -SellPrice WHEN Type='Surcharge' THEN SellPrice ELSE 0 END) AS TotalPrice FROM OrderCostings WHERE ItemId='" & itemId & "'")
            Dim buyPrice As Decimal = GetItemData_Decimal("SELECT SUM(CASE WHEN Type='Base' THEN BuyPrice WHEN Type='Discount' THEN -BuyPrice WHEN Type='Surcharge' THEN BuyPrice ELSE 0 END) AS TotalPrice FROM OrderCostings WHERE ItemId='" & itemId & "'")

            Dim dataCosting As Object() = {headerId, itemId, 0, "Final", "Final Cost This Item", buyPrice, sellPrice}
            OrderCostings(dataCosting)
        Catch ex As Exception
        End Try
    End Sub

    Public Sub CalculatePriceByOrder(headerId As String)
        Try
            Dim thisData As DataTable = GetDataTable("SELECT OrderDetails.Id FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND Designs.Type<>'Service'")
            If Not thisData.Rows.Count = 0 Then
                For i As Integer = 0 To thisData.Rows.Count - 1
                    Dim itemId As String = thisData.Rows(i)("Id").ToString()

                    ResetPriceDetail(headerId, itemId)
                    CalculatePrice(headerId, itemId)
                    FinalCostItem(headerId, itemId)
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub CalculatePrice(headerId As String, itemId As String)
        Try
            Dim thisData As DataRow = GetDataRow("SELECT * FROM OrderDetails WHERE Id='" & itemId & "' AND Active=1")
            If thisData IsNot Nothing Then
                Dim customerId As String = GetCustomerIdByOrder(headerId)

                Dim companyId As String = GetCompanyIdByOrder(headerId)
                Dim companyDetailId As String = GetCompanyDetailIdByOrder(headerId)

                Dim orderType As String = GetOrderType(headerId)

                Dim priceGroup As String = GetPriceGroupByOrder(headerId)
                Dim shutterPriceGroup As String = GetItemData("SELECT ShutterPriceGroupId FROM Customers WHERE Id='" & customerId & "'")
                Dim doorPriceGroup As String = GetItemData("SELECT DoorPriceGroupId FROM Customers WHERE Id='" & customerId & "'")

                Dim productId As String = thisData("ProductId").ToString()
                Dim designId As String = GetDesignId(productId)
                Dim blindId As String = GetBlindId(productId)
                Dim tubeId As String = GetTubeId(productId)

                Dim designName As String = GetDesignName(designId)
                Dim blindName As String = GetBlindName(blindId)
                Dim productName As String = GetProductName(productId)
                Dim tubeName As String = GetTubeName(tubeId)

                Dim fabricId As String = thisData("FabricId").ToString()
                Dim fabricIdB As String = thisData("FabricIdB").ToString()
                Dim fabricIdC As String = thisData("FabricIdC").ToString()
                Dim fabricIdD As String = thisData("FabricIdD").ToString()
                Dim fabricIdE As String = thisData("FabricIdE").ToString()
                Dim fabricIdF As String = thisData("FabricIdF").ToString()

                Dim fabricColourId As String = thisData("FabricColourId").ToString()
                Dim fabricColourIdB As String = thisData("FabricColourIdB").ToString()
                Dim fabricColourIdC As String = thisData("FabricColourIdC").ToString()
                Dim fabricColourIdD As String = thisData("FabricColourIdD").ToString()
                Dim fabricColourIdE As String = thisData("FabricColourIdE").ToString()
                Dim fabricColourIdF As String = thisData("FabricColourIdF").ToString()

                Dim priceProductGroupId As String = thisData("PriceProductGroupId").ToString()
                Dim priceProductGroupIdB As String = thisData("PriceProductGroupIdB").ToString()
                Dim priceProductGroupIdC As String = thisData("PriceProductGroupIdC").ToString()
                Dim priceProductGroupIdD As String = thisData("PriceProductGroupIdD").ToString()
                Dim priceProductGroupIdE As String = thisData("PriceProductGroupIdE").ToString()
                Dim priceProductGroupIdF As String = thisData("PriceProductGroupIdF").ToString()
                Dim priceProductGroupIdG As String = thisData("PriceProductGroupIdG").ToString()

                Dim priceProductGroupName As String = GetPriceProductGroupName(priceProductGroupId)
                Dim priceProductGroupNameB As String = GetPriceProductGroupName(priceProductGroupIdB)
                Dim priceProductGroupNameC As String = GetPriceProductGroupName(priceProductGroupIdC)
                Dim priceProductGroupNameD As String = GetPriceProductGroupName(priceProductGroupIdD)
                Dim priceProductGroupNameE As String = GetPriceProductGroupName(priceProductGroupIdE)
                Dim priceProductGroupNameF As String = GetPriceProductGroupName(priceProductGroupIdF)
                Dim priceProductGroupNameG As String = GetPriceProductGroupName(priceProductGroupIdG)

                Dim priceAdditional As String = thisData("PriceAdditional").ToString()
                Dim priceAdditionalB As String = thisData("PriceAdditionalB").ToString()

                Dim priceAdditionalName As String = GetPriceProductGroupName(priceAdditional)
                Dim priceAdditionalNameB As String = GetPriceProductGroupName(priceAdditionalB)

                Dim width As String = thisData("Width").ToString()
                Dim widthB As String = thisData("WidthB").ToString()
                Dim widthC As String = thisData("widthC").ToString()
                Dim widthD As String = thisData("WidthD").ToString()
                Dim widthE As String = thisData("WidthE").ToString()
                Dim widthF As String = thisData("WidthF").ToString()
                Dim widthG As String = thisData("WidthG").ToString()
                Dim widthH As String = thisData("WidthH").ToString()

                Dim drop As String = thisData("Drop").ToString()
                Dim dropB As String = thisData("DropB").ToString()
                Dim dropC As String = thisData("DropC").ToString()
                Dim dropD As String = thisData("DropD").ToString()
                Dim dropE As String = thisData("DropE").ToString()
                Dim dropF As String = thisData("DropF").ToString()
                Dim dropG As String = thisData("DropG").ToString()
                Dim dropH As String = thisData("DropH").ToString()

                Dim trackType As String = thisData("TrackType").ToString()
                Dim trackTypeb As String = thisData("TrackTypeB").ToString()

                Dim subType As String = thisData("SubType").ToString()
                Dim layoutCode As String = thisData("LayoutCode").ToString()
                Dim frameColour As String = thisData("FrameColour").ToString()

                Dim totalItems As Integer = thisData("TotalItems")

                Dim cutLength As Integer = 0
                If Not IsDBNull(thisData("LouvreSize")) Then
                    cutLength = thisData("LouvreSize")
                End If

                Dim linearMetre As Decimal = 0D
                If Not IsDBNull(thisData("LinearMetre")) Then
                    linearMetre = Math.Round(Convert.ToDecimal(thisData("LinearMetre")), 2)
                End If
                Dim linearMetreB As Decimal = 0D
                If Not IsDBNull(thisData("LinearMetreB")) Then
                    linearMetreB = Math.Round(Convert.ToDecimal(thisData("LinearMetreB")), 2)
                End If
                Dim linearMetreC As Decimal = 0D
                If Not IsDBNull(thisData("LinearMetreC")) Then
                    linearMetreC = Math.Round(Convert.ToDecimal(thisData("LinearMetreC")), 2)
                End If
                Dim linearMetreD As Decimal = 0D
                If Not IsDBNull(thisData("LinearMetreD")) Then
                    linearMetreD = Math.Round(Convert.ToDecimal(thisData("LinearMetreD")), 2)
                End If
                Dim linearMetreE As Decimal = 0D
                If Not IsDBNull(thisData("LinearMetreE")) Then
                    linearMetreE = Math.Round(Convert.ToDecimal(thisData("LinearMetreE")), 2)
                End If
                Dim linearMetreF As Decimal = 0D
                If Not IsDBNull(thisData("LinearMetreF")) Then
                    linearMetreF = Math.Round(Convert.ToDecimal(thisData("LinearMetreF")), 2)
                End If

                Dim squareMetre As Decimal = 0D
                If Not IsDBNull(thisData("SquareMetre")) Then
                    squareMetre = Math.Round(Convert.ToDecimal(thisData("SquareMetre")), 2)
                End If
                Dim squareMetreB As Decimal = 0D
                If Not IsDBNull(thisData("SquareMetreB")) Then
                    squareMetreB = Math.Round(Convert.ToDecimal(thisData("SquareMetreB")), 2)
                End If
                Dim squareMetreC As Decimal = 0D
                If Not IsDBNull(thisData("SquareMetreC")) Then
                    squareMetreC = Math.Round(Convert.ToDecimal(thisData("SquareMetreC")), 2)
                End If
                Dim squareMetreD As Decimal = 0D
                If Not IsDBNull(thisData("SquareMetreD")) Then
                    squareMetreD = Math.Round(Convert.ToDecimal(thisData("SquareMetreD")), 2)
                End If
                Dim squareMetreE As Decimal = 0D
                If Not IsDBNull(thisData("SquareMetreE")) Then
                    squareMetreE = Math.Round(Convert.ToDecimal(thisData("SquareMetreE")), 2)
                End If
                Dim squareMetreF As Decimal = 0D
                If Not IsDBNull(thisData("SquareMetreF")) Then
                    squareMetreF = Math.Round(Convert.ToDecimal(thisData("SquareMetreF")), 2)
                End If

                'Dim objectArray As Object() = Nothing

                Dim itemNumber As Integer = 0

                Dim thisSell As Decimal = 0
                Dim thisSellAdditional As Decimal = 0
                Dim thisBuy As Decimal = 0
                Dim thisBuyAdditional As Decimal = 0

                Dim customPricing As String = GetItemData("SELECT Description FROM CustomerCustomPricings WHERE Id='" & customerId & "'")
                Dim isWithoutGR As Boolean = Not String.IsNullOrEmpty(customPricing) AndAlso customPricing.Contains("Without GR")

                ' FIRST BLIND
                If Not String.IsNullOrEmpty(priceProductGroupId) Then
                    itemNumber = 1

                    Dim sellArray As Object() = {priceProductGroupId, priceGroup, drop, width, "Sell"}
                    If designName = "Skyline Shutter Express" Then
                        sellArray = {priceProductGroupId, shutterPriceGroup, drop, width, "Sell"}
                    End If
                    If designName = "Skyline Shutter Ocean" Then
                        sellArray = {priceProductGroupId, shutterPriceGroup, drop, width, "Sell"}
                    End If
                    If designName = "Door" Then
                        sellArray = {priceProductGroupId, doorPriceGroup, drop, width, "Sell"}
                    End If
                    If designName = "Window" Then
                        sellArray = {priceProductGroupId, doorPriceGroup, drop, width, "Sell"}
                    End If
                    If designName = "Outdoor" Then
                        sellArray = {priceProductGroupId, priceGroup, squareMetre, squareMetre, "Sell"}
                    End If

                    Dim buyArray As Object() = {priceProductGroupId, priceGroup, drop, width, "Buy"}
                    If designName = "Skyline Shutter Express" Then
                        buyArray = {priceProductGroupId, shutterPriceGroup, drop, width, "Buy"}
                    End If
                    If designName = "Skyline Shutter Ocean" Then
                        buyArray = {priceProductGroupId, shutterPriceGroup, drop, width, "Buy"}
                    End If
                    If designName = "Door" Then
                        buyArray = {priceProductGroupId, doorPriceGroup, drop, width, "Buy"}
                    End If
                    If designName = "Window" Then
                        buyArray = {priceProductGroupId, doorPriceGroup, drop, width, "Buy"}
                    End If

                    Dim dataPriceSell As DataRow = GetGridPrice(sellArray)
                    Dim dataPriceBuy As DataRow = GetGridPrice(buyArray)

                    Dim gridSellPrice As Decimal = 0D
                    Dim gridBuyPrice As Decimal = 0D

                    Dim gridSellMethod As String = String.Empty
                    Dim gridBuyMethod As String = String.Empty

                    Dim gridSellConditional As String = String.Empty
                    Dim gridBuyConditional As String = String.Empty

                    If dataPriceSell IsNot Nothing Then
                        gridSellMethod = dataPriceSell("Method").ToString()
                        gridSellPrice = Convert.ToDecimal(dataPriceSell("Price"))
                        gridSellConditional = dataPriceSell("Conditional").ToString()
                    End If
                    If dataPriceBuy IsNot Nothing Then
                        gridBuyMethod = dataPriceBuy("Method").ToString()
                        gridBuyPrice = Convert.ToDecimal(dataPriceBuy("Price"))
                        gridBuyConditional = dataPriceBuy("Conditional").ToString()
                    End If

                    Dim gridSellAdditional As Decimal = 0D
                    Dim gridBuyAdditional As Decimal = 0D

                    If Not String.IsNullOrEmpty(priceAdditional) Then
                        Dim additionalArray As Object() = {priceAdditional, priceGroup, 0, width, "Sell"}
                        Dim addData As DataRow = GetGridPrice(additionalArray)
                        If addData IsNot Nothing Then
                            gridSellAdditional = addData("Price")
                        End If
                    End If
                    If Not String.IsNullOrEmpty(priceAdditional) Then
                        Dim additionalArray As Object() = {priceAdditional, priceGroup, 0, width, "Buy"}
                        Dim addData As DataRow = GetGridPrice(additionalArray)
                        If addData IsNot Nothing Then
                            gridBuyAdditional = addData("Price")
                        End If
                    End If

                    Dim costSell As Decimal = gridSellPrice
                    Dim costSellAdditional As Decimal = gridSellAdditional

                    Dim costBuy As Decimal = gridBuyPrice
                    Dim costBuyAdditional As Decimal = gridBuyAdditional

                    thisSell = costSell
                    thisSellAdditional = costSellAdditional

                    thisBuy = costBuy
                    thisBuyAdditional = costBuyAdditional

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END ASC")
                    For Each discountRow As DataRow In discountData.Rows
                        Dim discountType As String = discountRow("Type").ToString()
                        Dim dataId As String = discountRow("DataId").ToString()
                        Dim discount As Decimal = CDec(discountRow("Discount"))

                        If (discountType = "Designs" AndAlso dataId <> designId) OrElse (discountType = "PriceProductGroups" AndAlso dataId <> priceProductGroupId) Then
                            Continue For
                        End If

                        Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                        thisSell = Math.Round(costSell - discountValue, 2)
                        If (designName = "Roller Blind" OrElse priceProductGroupName.Contains("Gear Reduction")) Then
                            discountValue = Math.Round((costSell - 7D) * discount / 100D, 2)

                            If isWithoutGR Then
                                thisSell = Math.Round(costSell - 7D - discountValue, 2)
                            ElseIf gridSellConditional = "Excl. $7 Disc" Then
                                thisSell = Math.Round((costSell - 7D) - discountValue + 7D, 2)
                            End If
                        End If

                        costSell = thisSell

                        Dim discountAdditionalValue As Decimal = Math.Round(costSellAdditional * discount / 100D, 2)
                        thisSellAdditional = Math.Round(costSellAdditional - discountAdditionalValue, 2)
                        costSellAdditional = thisSellAdditional
                    Next

                    If designName = "Curtain" AndAlso orderType = "Builder" Then
                        thisSell = thisSell + 50
                    End If

                    If gridSellMethod = "Square Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            thisSell = Math.Round(thisSell * squareMetre, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            If squareMetre < 1 Then thisSell = thisSell * 1
                            If squareMetre >= 1 Then thisSell = Math.Round(thisSell * squareMetre, 2)
                        End If
                        thisBuy = Math.Round(thisBuy * squareMetre, 2)
                    End If
                    If gridSellMethod = "Linear Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            thisSell = Math.Round(thisSell * linearMetre, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            If linearMetre < 1 Then thisSell = thisSell * 1
                            If linearMetre >= 1 Then thisSell = Math.Round(thisSell * linearMetre, 2)

                            If designName = "Curtain" AndAlso (blindName = "Complete Set (Single)" OrElse blindName = "Curtain Only") Then
                                thisSell = Math.Round(cutLength / 1000 * costSell, 2)
                            End If
                        End If
                        thisBuy = Math.Round(thisBuy * linearMetre, 2)
                    End If

                    If designName = "Skyline Shutter Express" Then
                        If companyId = "2" Then
                            If squareMetre <= 0.75 Then thisSell = Math.Round(gridSellPrice * 0.75, 2)
                            If squareMetre <= 0.5 Then thisBuy = Math.Round(gridBuyPrice * 0.5, 2)
                        End If
                        If companyId = "3" Then
                            If squareMetre <= 0.5 Then thisSell = Math.Round(gridSellPrice * 0.5, 2)
                        End If
                    End If
                    If designName = "Skyline Shutter Ocean" Then
                        If squareMetre <= 0.75 Then thisSell = Math.Round(gridSellPrice * 0.75, 2)
                    End If

                    Dim buyPromoData As DataTable = GetDataTable("SELECT Id FROM Promos WHERE Active=1 AND Type='Buy' AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each buyPromoRow As DataRow In buyPromoData.Rows
                        Dim promoId As String = buyPromoRow("Id").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim isMatch As Boolean = False

                            Select Case promoType
                                Case "Designs"
                                    isMatch = (dataId = designId)
                                Case "Blinds"
                                    isMatch = (dataId = blindId)
                                Case "Products"
                                    isMatch = (dataId = productId)
                                Case "Fabrics"
                                    isMatch = ((designName = "Roller Blind" OrElse designName = "Curtain") AndAlso dataId = fabricId)
                                Case "FabricColours"
                                    isMatch = ((designName = "Roller Blind" OrElse designName = "Curtain") AndAlso dataId = fabricColourId)
                                Case "FrameColours"
                                    isMatch = (dataId = frameColour)
                            End Select

                            If Not isMatch Then Continue For

                            Dim promoValue As Decimal = Math.Round(costBuy * discount / 100D, 2)
                            thisBuy = Math.Round(costBuy - promoValue, 2)
                            costBuy = thisBuy
                        Next
                    Next

                    Dim sellPromoData As DataTable = GetDataTable("SELECT CustomerPromos.PromoId FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each sellPromoRow As DataRow In sellPromoData.Rows
                        Dim promoId As String = sellPromoRow("PromoId").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim compareId As String = ""

                            Select Case promoType
                                Case "Designs"
                                    compareId = designId
                                Case "Blinds"
                                    compareId = blindId
                                Case "Products"
                                    compareId = productId
                                Case "Fabrics"
                                    If designName = "Roller Blind" OrElse designName = "Curtain" Then compareId = fabricId
                                Case "FabricColours"
                                    If designName = "Roller Blind" OrElse designName = "Curtain" Then compareId = fabricColourId
                                Case "FrameColours"
                                    compareId = frameColour
                            End Select

                            If compareId <> dataId Then Continue For

                            Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                            thisSell = Math.Round(costSell - discountValue, 2)
                            costSell = thisSell
                        Next
                    Next

                    Dim costingDescription As String = priceProductGroupName
                    If totalItems > 1 AndAlso Not (designName = "Skyline Shutter Express" OrElse designName = "Skyline Shutter Ocean" OrElse designName = "Evolve Shutter Express" OrElse designName = "Evolve Shutter Ocean") Then
                        costingDescription = String.Format("#1 {0}", priceProductGroupName)
                    End If

                    If designName = "Door" AndAlso (tubeName = "Hinged Double" OrElse tubeName = "Sliding Double") Then
                        costingDescription = String.Format("#1 {0}", priceProductGroupName)
                    End If

                    Dim costingArray As Object() = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    OrderCostings(costingArray)

                    If designName = "Curtain" Then
                        If blindName = "Complete Set (Single)" Then
                            costingDescription = priceAdditionalName
                        End If
                        If blindName = "Complete Set (Double)" Then
                            costingDescription = String.Format("#1 {0}", priceAdditionalName)
                        End If
                        costingArray = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuyAdditional, thisSellAdditional}
                        OrderCostings(costingArray)
                    End If

                    costingArray = {headerId, itemId, itemNumber, priceGroup}
                    If designName = "Skyline Shutter Express" Then
                        costingArray = {headerId, itemId, itemNumber, shutterPriceGroup}
                    End If
                    If designName = "Skyline Shutter Ocean" Then
                        costingArray = {headerId, itemId, itemNumber, shutterPriceGroup}
                    End If
                    If designName = "Door" Then
                        costingArray = {headerId, itemId, itemNumber, doorPriceGroup}
                    End If
                    If designName = "Window" Then
                        costingArray = {headerId, itemId, itemNumber, doorPriceGroup}
                    End If

                    Dim surchargeArray As Object() = {headerId, itemId, designId, itemNumber, priceGroup}
                    If designName = "Skyline Shutter Express" Then
                        surchargeArray = {headerId, itemId, designId, itemNumber, shutterPriceGroup}
                    End If
                    If designName = "Skyline Shutter Ocean" Then
                        surchargeArray = {headerId, itemId, designId, itemNumber, shutterPriceGroup}
                    End If
                    If designName = "Door" Then
                        surchargeArray = {headerId, itemId, designId, itemNumber, doorPriceGroup}
                    End If
                    If designName = "Window" Then
                        surchargeArray = {headerId, itemId, designId, itemNumber, doorPriceGroup}
                    End If
                    CalculateSurcharge(surchargeArray)
                End If

                ' SECOND BLIND
                If Not String.IsNullOrEmpty(priceProductGroupIdB) Then
                    itemNumber = 2

                    Dim sellArray As Object() = {priceProductGroupIdB, priceGroup, dropB, widthB, "Sell"}
                    If designName = "Door" Then
                        sellArray = {priceProductGroupIdB, doorPriceGroup, drop, width, "Sell"}
                    End If

                    Dim buyArray As Object() = {priceProductGroupIdB, priceGroup, dropB, widthB, "Buy"}
                    If designName = "Door" Then
                        buyArray = {priceProductGroupIdB, doorPriceGroup, drop, width, "Buy"}
                    End If

                    Dim dataPriceSell As DataRow = GetGridPrice(sellArray)
                    Dim dataPriceBuy As DataRow = GetGridPrice(buyArray)

                    Dim gridSellPrice As Decimal = 0D
                    Dim gridBuyPrice As Decimal = 0D

                    Dim gridSellMethod As String = String.Empty
                    Dim gridBuyMethod As String = String.Empty

                    Dim gridSellConditional As String = String.Empty
                    Dim gridBuyConditional As String = String.Empty

                    If Not dataPriceSell Is Nothing Then
                        gridSellMethod = dataPriceSell("Method").ToString()
                        gridSellPrice = dataPriceSell("Price")
                        gridSellConditional = dataPriceSell("Conditional").ToString()
                    End If

                    If Not dataPriceBuy Is Nothing Then
                        gridBuyMethod = dataPriceSell("Method").ToString()
                        gridBuyPrice = dataPriceSell("Price")
                        gridBuyConditional = dataPriceSell("Conditional").ToString()
                    End If

                    Dim gridSellAdditional As Decimal = 0D
                    Dim gridBuyAdditional As Decimal = 0D

                    If Not String.IsNullOrEmpty(priceAdditionalB) Then
                        Dim additionalArray As Object() = {priceAdditionalB, priceGroup, 0, width, "Sell"}
                        Dim addData As DataRow = GetGridPrice(additionalArray)
                        If addData IsNot Nothing Then
                            gridSellAdditional = addData("Price")
                        End If
                    End If
                    If Not String.IsNullOrEmpty(priceAdditionalB) Then
                        Dim additionalArray As Object() = {priceAdditionalB, priceGroup, 0, width, "Buy"}
                        Dim addData As DataRow = GetGridPrice(additionalArray)
                        If addData IsNot Nothing Then
                            gridBuyAdditional = addData("Price")
                        End If
                    End If

                    Dim costSell As Decimal = gridSellPrice
                    Dim costSellAdditional As Decimal = gridSellAdditional

                    Dim costBuy As Decimal = gridBuyPrice
                    Dim costBuyAdditional As Decimal = gridBuyAdditional

                    thisSell = costSell
                    thisSellAdditional = costSellAdditional

                    thisBuy = costBuy
                    thisBuyAdditional = costBuyAdditional

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END ASC")
                    For Each discountRow As DataRow In discountData.Rows
                        Dim discountType As String = discountRow("Type").ToString()
                        Dim dataId As String = discountRow("DataId").ToString()
                        Dim discount As Decimal = CDec(discountRow("Discount"))

                        If (discountType = "Designs" AndAlso dataId <> designId) OrElse (discountType = "PriceProductGroups" AndAlso dataId <> priceProductGroupIdB) Then
                            Continue For
                        End If

                        Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                        thisSell = Math.Round(costSell - discountValue, 2)
                        If (designName = "Roller Blind" OrElse priceProductGroupNameB.Contains("Gear Reduction")) Then
                            discountValue = Math.Round((costSell - 7D) * discount / 100D, 2)

                            If isWithoutGR Then
                                thisSell = Math.Round(costSell - 7D - discountValue, 2)
                            ElseIf gridSellConditional = "Excl. $7 Disc" Then
                                thisSell = Math.Round((costSell - 7D) - discountValue + 7D, 2)
                            End If
                        End If

                        costSell = thisSell

                        Dim discountAdditionalValue As Decimal = Math.Round(costSellAdditional * discount / 100D, 2)
                        thisSellAdditional = Math.Round(costSellAdditional - discountAdditionalValue, 2)
                        costSellAdditional = thisSellAdditional
                    Next

                    If designName = "Curtain" AndAlso orderType = "Builder" Then
                        thisSell = thisSell + 50
                    End If

                    If gridSellMethod = "Square Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            thisSell = Math.Round(thisSell * squareMetreB, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            If squareMetre < 1 Then thisSell = thisSell * 1
                            If squareMetre >= 1 Then thisSell = Math.Round(thisSell * squareMetreB, 2)
                        End If
                        thisBuy = Math.Round(thisBuy * squareMetreB, 2)
                    End If
                    If gridSellMethod = "Linear Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            thisSell = Math.Round(thisSell * linearMetreB, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            If linearMetre < 1 Then thisSell = thisSell * 1
                            If linearMetre >= 1 Then thisSell = Math.Round(thisSell * linearMetreB, 2)
                        End If
                        thisBuy = Math.Round(thisBuy * linearMetreB, 2)
                    End If

                    Dim buyPromoData As DataTable = GetDataTable("SELECT Id FROM Promos WHERE Active=1 AND Type='Buy' AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each buyPromoRow As DataRow In buyPromoData.Rows
                        Dim promoId As String = buyPromoRow("Id").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim isMatch As Boolean = False

                            Select Case promoType
                                Case "Designs"
                                    isMatch = (dataId = designId)
                                Case "Blinds"
                                    isMatch = (dataId = blindId)
                                Case "Products"
                                    isMatch = (dataId = productId)
                                Case "Fabrics"
                                    isMatch = ((designName = "Roller Blind" OrElse designName = "Curtain") AndAlso dataId = fabricIdB)
                                Case "FabricColours"
                                    isMatch = ((designName = "Roller Blind" OrElse designName = "Curtain") AndAlso dataId = fabricColourIdB)
                                Case "FrameColours"
                                    isMatch = (dataId = frameColour)
                            End Select

                            If Not isMatch Then Continue For

                            Dim promoValue As Decimal = Math.Round(costBuy * discount / 100D, 2)
                            thisBuy = Math.Round(costBuy - promoValue, 2)
                            costBuy = thisBuy
                        Next
                    Next

                    Dim sellPromoData As DataTable = GetDataTable("SELECT CustomerPromos.PromoId FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each sellPromoRow As DataRow In sellPromoData.Rows
                        Dim promoId As String = sellPromoRow("PromoId").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim compareId As String = ""

                            Select Case promoType
                                Case "Designs"
                                    compareId = designId
                                Case "Blinds"
                                    compareId = blindId
                                Case "Products"
                                    compareId = productId
                                Case "Fabrics"
                                    If designName = "Roller Blind" OrElse designName = "Curtain" Then compareId = fabricIdB
                                Case "FabricColours"
                                    If designName = "Roller Blind" OrElse designName = "Curtain" Then compareId = fabricColourIdB
                                Case "FrameColours"
                                    compareId = frameColour
                            End Select

                            If compareId <> dataId Then Continue For

                            Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                            thisSell = Math.Round(costSell - discountValue, 2)
                            costSell = thisSell
                        Next
                    Next

                    Dim costingDescription As String = String.Format("#2 {0}", priceProductGroupNameB)
                    If designName = "Cellular Shades" Then
                        costingDescription = priceProductGroupNameB
                    End If

                    Dim costingArray As Object() = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    OrderCostings(costingArray)

                    If designName = "Curtain" AndAlso blindName = "Complete Set (Double)" Then
                        costingDescription = String.Format("#2 {0}", priceAdditionalNameB)
                        costingArray = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuyAdditional, thisSellAdditional}
                        OrderCostings(costingArray)
                    End If

                    Dim surchargeArray As Object() = {headerId, itemId, itemNumber, priceGroup}
                    If designName = "Door" Then
                        surchargeArray = {headerId, itemId, itemNumber, doorPriceGroup}
                    End If

                    surchargeArray = {headerId, itemId, designId, itemNumber, priceGroup}
                    CalculateSurcharge(surchargeArray)
                End If

                ' THIRD BLIND
                If Not String.IsNullOrEmpty(priceProductGroupIdC) Then
                    itemNumber = 3

                    Dim sellArray As Object() = {priceProductGroupIdC, priceGroup, dropC, widthC, "Sell"}
                    Dim buyArray As Object() = {priceProductGroupIdC, priceGroup, dropC, widthC, "Buy"}

                    Dim dataPriceSell As DataRow = GetGridPrice(sellArray)
                    Dim dataPriceBuy As DataRow = GetGridPrice(buyArray)

                    Dim gridSellPrice As Decimal = 0D
                    Dim gridBuyPrice As Decimal = 0D

                    Dim gridSellMethod As String = String.Empty
                    Dim gridBuyMethod As String = String.Empty

                    Dim gridSellConditional As String = String.Empty
                    Dim gridBuyConditional As String = String.Empty

                    If Not dataPriceSell Is Nothing Then
                        gridSellMethod = dataPriceSell("Method").ToString()
                        gridSellPrice = dataPriceSell("Price")
                        gridSellConditional = dataPriceSell("Conditional").ToString()
                    End If

                    If Not dataPriceBuy Is Nothing Then
                        gridBuyMethod = dataPriceBuy("Method").ToString()
                        gridBuyPrice = dataPriceBuy("Price")
                        gridBuyConditional = dataPriceBuy("Conditional").ToString()
                    End If

                    Dim costSell As Decimal = gridSellPrice
                    Dim costBuy As Decimal = gridBuyPrice

                    thisSell = costSell
                    thisBuy = costBuy

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END ASC")
                    For Each discountRow As DataRow In discountData.Rows
                        Dim discountType As String = discountRow("Type").ToString()
                        Dim dataId As String = discountRow("DataId").ToString()
                        Dim discount As Decimal = CDec(discountRow("Discount"))

                        If (discountType = "Designs" AndAlso dataId <> designId) OrElse (discountType = "PriceProductGroups" AndAlso dataId <> priceProductGroupIdC) Then
                            Continue For
                        End If

                        Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                        thisSell = Math.Round(costSell - discountValue, 2)
                        If (designName = "Roller Blind" OrElse priceProductGroupNameC.Contains("Gear Reduction")) Then
                            discountValue = Math.Round((costSell - 7D) * discount / 100D, 2)

                            If isWithoutGR Then
                                thisSell = Math.Round(costSell - 7D - discountValue, 2)
                            ElseIf gridSellConditional = "Excl. $7 Disc" Then
                                thisSell = Math.Round((costSell - 7D) - discountValue + 7D, 2)
                            End If
                        End If

                        costSell = thisSell
                    Next

                    If gridSellMethod = "Square Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            If squareMetreC < 1 Then thisSell = thisSell * 1
                            If squareMetreC >= 1 Then thisSell = Math.Round(thisSell * squareMetreC, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            thisSell = Math.Round(thisSell * squareMetreC, 2)
                        End If
                        thisBuy = Math.Round(thisBuy * squareMetreC, 2)
                    End If
                    If gridSellMethod = "Linear Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            thisSell = Math.Round(thisSell * linearMetreC, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            If linearMetreC < 1 Then thisSell = thisSell * 1
                            If linearMetreC < 1 Then thisSell = Math.Round(thisSell * linearMetreC, 2)
                        End If
                        thisBuy = Math.Round(thisBuy * linearMetreC, 2)
                    End If

                    Dim buyPromoData As DataTable = GetDataTable("SELECT Id FROM Promos WHERE Active=1 AND Type='Buy' AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each buyPromoRow As DataRow In buyPromoData.Rows
                        Dim promoId As String = buyPromoRow("Id").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim isMatch As Boolean = False

                            Select Case promoType
                                Case "Designs"
                                    isMatch = (dataId = designId)
                                Case "Blinds"
                                    isMatch = (dataId = blindId)
                                Case "Products"
                                    isMatch = (dataId = productId)
                                Case "Fabrics"
                                    isMatch = (designName = "Roller Blind" AndAlso dataId = fabricIdC)
                                Case "FabricColours"
                                    isMatch = (designName = "Roller Blind" AndAlso dataId = fabricColourIdC)
                            End Select

                            If Not isMatch Then Continue For

                            Dim promoValue As Decimal = Math.Round(costBuy * discount / 100D, 2)
                            thisBuy = Math.Round(costBuy - promoValue, 2)
                            costBuy = thisBuy
                        Next
                    Next

                    Dim sellPromoData As DataTable = GetDataTable("SELECT CustomerPromos.PromoId FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each sellPromoRow As DataRow In sellPromoData.Rows
                        Dim promoId As String = sellPromoRow("PromoId").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim compareId As String = ""

                            Select Case promoType
                                Case "Designs"
                                    compareId = designId
                                Case "Blinds"
                                    compareId = blindId
                                Case "Products"
                                    compareId = productId
                                Case "Fabrics"
                                    If designName = "Roller Blind" Then compareId = fabricIdC
                                Case "FabricColours"
                                    If designName = "Roller Blind" Then compareId = fabricColourIdC
                            End Select

                            If compareId <> dataId Then Continue For

                            Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                            thisSell = Math.Round(costSell - discountValue, 2)
                            costSell = thisSell
                        Next
                    Next

                    Dim costingDescription As String = String.Format("#3 {0}", priceProductGroupNameC)
                    Dim costingArray As Object() = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    Dim surchargeArray As Object() = {headerId, itemId, designId, itemNumber, priceGroup}

                    OrderCostings(costingArray)
                    CalculateSurcharge(surchargeArray)
                End If

                ' FOURTH BLIND
                If Not String.IsNullOrEmpty(priceProductGroupIdD) Then
                    itemNumber = 4

                    Dim sellArray As Object() = {priceProductGroupIdD, priceGroup, dropD, widthD, "Sell"}
                    Dim buyArray As Object() = {priceProductGroupIdD, priceGroup, dropD, widthD, "Buy"}

                    Dim dataPriceSell As DataRow = GetGridPrice(sellArray)
                    Dim dataPriceBuy As DataRow = GetGridPrice(buyArray)

                    Dim gridSellPrice As Decimal = 0D
                    Dim gridBuyPrice As Decimal = 0D

                    Dim gridSellMethod As String = String.Empty
                    Dim gridBuyMethod As String = String.Empty

                    Dim gridSellConditional As String = String.Empty
                    Dim gridBuyConditional As String = String.Empty

                    If Not dataPriceSell Is Nothing Then
                        gridSellMethod = dataPriceSell("Method").ToString()
                        gridSellPrice = dataPriceSell("Price")
                        gridSellConditional = dataPriceSell("Conditional").ToString()
                    End If
                    If Not dataPriceBuy Is Nothing Then
                        gridBuyMethod = dataPriceBuy("Method").ToString()
                        gridBuyPrice = dataPriceBuy("Price")
                        gridBuyConditional = dataPriceBuy("Conditional").ToString()
                    End If

                    Dim costSell As Decimal = gridSellPrice
                    Dim costBuy As Decimal = gridBuyPrice

                    thisSell = costSell
                    thisBuy = costBuy

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END ASC")
                    For Each discountRow As DataRow In discountData.Rows
                        Dim discountType As String = discountRow("Type").ToString()
                        Dim dataId As String = discountRow("DataId").ToString()
                        Dim discount As Decimal = CDec(discountRow("Discount"))

                        If (discountType = "Designs" AndAlso dataId <> designId) OrElse (discountType = "PriceProductGroups" AndAlso dataId <> priceProductGroupIdD) Then
                            Continue For
                        End If

                        Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                        thisSell = Math.Round(costSell - discountValue, 2)
                        If (designName = "Roller Blind" OrElse priceProductGroupNameD.Contains("Gear Reduction")) Then
                            discountValue = Math.Round((costSell - 7D) * discount / 100D, 2)

                            If isWithoutGR Then
                                thisSell = Math.Round(costSell - 7D - discountValue, 2)
                            ElseIf gridSellConditional = "Excl. $7 Disc" Then
                                thisSell = Math.Round((costSell - 7D) - discountValue + 7D, 2)
                            End If
                        End If

                        costSell = thisSell
                    Next

                    If gridSellMethod = "Square Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            thisSell = Math.Round(thisSell * squareMetreD, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            If squareMetreD < 1 Then thisSell = thisSell * 1
                            If squareMetreD >= 1 Then thisSell = Math.Round(thisSell * squareMetreD, 2)
                        End If
                        thisBuy = Math.Round(thisBuy * squareMetreD, 2)
                    End If
                    If gridSellMethod = "Linear Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            thisSell = Math.Round(thisSell * linearMetreD, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            If linearMetreD < 1 Then thisSell = thisSell * 1
                            If linearMetreD >= 1 Then thisSell = Math.Round(thisSell * linearMetreD, 2)
                        End If
                        thisBuy = Math.Round(thisBuy * linearMetreD, 2)
                    End If

                    Dim buyPromoData As DataTable = GetDataTable("SELECT Id FROM Promos WHERE Active=1 AND Type='Buy' AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each buyPromoRow As DataRow In buyPromoData.Rows
                        Dim promoId As String = buyPromoRow("Id").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim isMatch As Boolean = False

                            Select Case promoType
                                Case "Designs"
                                    isMatch = (dataId = designId)
                                Case "Blinds"
                                    isMatch = (dataId = blindId)
                                Case "Products"
                                    isMatch = (dataId = productId)
                                Case "Fabrics"
                                    isMatch = (designName = "Roller Blind" AndAlso dataId = fabricIdD)
                                Case "FabricColours"
                                    isMatch = (designName = "Roller Blind" AndAlso dataId = fabricColourIdD)
                            End Select

                            If Not isMatch Then Continue For

                            Dim promoValue As Decimal = Math.Round(costBuy * discount / 100D, 2)
                            thisBuy = Math.Round(costBuy - promoValue, 2)
                            costBuy = thisBuy
                        Next
                    Next

                    Dim sellPromoData As DataTable = GetDataTable("SELECT CustomerPromos.PromoId FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each sellPromoRow As DataRow In sellPromoData.Rows
                        Dim promoId As String = sellPromoRow("PromoId").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim compareId As String = ""

                            Select Case promoType
                                Case "Designs"
                                    compareId = designId
                                Case "Blinds"
                                    compareId = blindId
                                Case "Products"
                                    compareId = productId
                                Case "Fabrics"
                                    If designName = "Roller Blind" Then compareId = fabricIdD
                                Case "FabricColours"
                                    If designName = "Roller Blind" Then compareId = fabricColourIdD
                            End Select

                            If compareId <> dataId Then Continue For

                            Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                            thisSell = Math.Round(costSell - discountValue, 2)
                            costSell = thisSell
                        Next
                    Next

                    Dim costingDescription As String = String.Format("#4 {0}", priceProductGroupNameD)
                    Dim costingArray As Object() = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    Dim surchargeArray As Object() = {headerId, itemId, designId, itemNumber, priceGroup}

                    OrderCostings(costingArray)
                    CalculateSurcharge(surchargeArray)
                End If

                'FIFTH BLIND
                If Not String.IsNullOrEmpty(priceProductGroupIdE) Then
                    itemNumber = 5

                    Dim sellArray As Object() = {priceProductGroupIdE, priceGroup, dropE, widthE, "Sell"}
                    Dim buyArray As Object() = {priceProductGroupIdE, priceGroup, dropE, widthE, "Buy"}

                    Dim dataPriceSell As DataRow = GetGridPrice(sellArray)
                    Dim dataPriceBuy As DataRow = GetGridPrice(buyArray)

                    Dim gridSellPrice As Decimal = 0D
                    Dim gridBuyPrice As Decimal = 0D

                    Dim gridSellMethod As String = String.Empty
                    Dim gridBuyMethod As String = String.Empty

                    Dim gridSellConditional As String = String.Empty
                    Dim gridBuyConditional As String = String.Empty

                    If Not dataPriceSell Is Nothing Then
                        gridSellMethod = dataPriceSell("Method").ToString()
                        gridSellPrice = dataPriceSell("Price")
                        gridSellConditional = dataPriceSell("Conditional").ToString()
                    End If
                    If Not dataPriceBuy Is Nothing Then
                        gridBuyMethod = dataPriceBuy("Method").ToString()
                        gridBuyPrice = dataPriceBuy("Price")
                        gridBuyConditional = dataPriceBuy("Conditional").ToString()
                    End If

                    Dim costSell As Decimal = gridSellPrice
                    Dim costBuy As Decimal = gridBuyPrice

                    thisSell = costSell
                    thisBuy = costBuy

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END ASC")
                    For Each discountRow As DataRow In discountData.Rows
                        Dim discountType As String = discountRow("Type").ToString()
                        Dim dataId As String = discountRow("DataId").ToString()
                        Dim discount As Decimal = CDec(discountRow("Discount"))

                        If (discountType = "Designs" AndAlso dataId <> designId) OrElse (discountType = "PriceProductGroups" AndAlso dataId <> priceProductGroupIdE) Then
                            Continue For
                        End If

                        Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                        thisSell = Math.Round(costSell - discountValue, 2)
                        If (designName = "Roller Blind" OrElse priceProductGroupNameE.Contains("Gear Reduction")) Then
                            discountValue = Math.Round((costSell - 7D) * discount / 100D, 2)

                            If isWithoutGR Then
                                thisSell = Math.Round(costSell - 7D - discountValue, 2)
                            ElseIf gridSellConditional = "Excl. $7 Disc" Then
                                thisSell = Math.Round((costSell - 7D) - discountValue + 7D, 2)
                            End If
                        End If

                        costSell = thisSell
                    Next

                    If gridSellMethod = "Square Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            thisSell = Math.Round(thisSell * squareMetreE, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            If squareMetreE < 1 Then thisSell = thisSell * 1
                            If squareMetreE >= 1 Then thisSell = Math.Round(thisSell * squareMetreE, 2)
                        End If
                        thisBuy = Math.Round(thisBuy * squareMetreE, 2)
                    End If
                    If gridSellMethod = "Linear Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            thisSell = Math.Round(thisSell * linearMetreE, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            If linearMetreE < 1 Then thisSell = thisSell * 1
                            If linearMetreE >= 1 Then thisSell = Math.Round(thisSell * linearMetreE, 2)
                        End If
                        thisBuy = Math.Round(thisBuy * linearMetreE, 2)
                    End If

                    Dim buyPromoData As DataTable = GetDataTable("SELECT Id FROM Promos WHERE Active=1 AND Type='Buy' AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each buyPromoRow As DataRow In buyPromoData.Rows
                        Dim promoId As String = buyPromoRow("Id").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim isMatch As Boolean = False

                            Select Case promoType
                                Case "Designs"
                                    isMatch = (dataId = designId)
                                Case "Blinds"
                                    isMatch = (dataId = blindId)
                                Case "Products"
                                    isMatch = (dataId = productId)
                                Case "Fabrics"
                                    isMatch = (designName = "Roller Blind" AndAlso dataId = fabricIdE)
                                Case "FabricColours"
                                    isMatch = (designName = "Roller Blind" AndAlso dataId = fabricColourIdE)
                            End Select

                            If Not isMatch Then Continue For

                            Dim promoValue As Decimal = Math.Round(costBuy * discount / 100D, 2)
                            thisBuy = Math.Round(costBuy - promoValue, 2)
                            costBuy = thisBuy
                        Next
                    Next

                    Dim sellPromoData As DataTable = GetDataTable("SELECT CustomerPromos.PromoId FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each sellPromoRow As DataRow In sellPromoData.Rows
                        Dim promoId As String = sellPromoRow("PromoId").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim compareId As String = ""

                            Select Case promoType
                                Case "Designs"
                                    compareId = designId
                                Case "Blinds"
                                    compareId = blindId
                                Case "Products"
                                    compareId = productId
                                Case "Fabrics"
                                    If designName = "Roller Blind" Then compareId = fabricIdE
                                Case "FabricColours"
                                    If designName = "Roller Blind" Then compareId = fabricColourIdE
                            End Select

                            If compareId <> dataId Then Continue For

                            Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                            thisSell = Math.Round(costSell - discountValue, 2)
                            costSell = thisSell
                        Next
                    Next

                    Dim costingDescription As String = String.Format("#5 {0}", priceProductGroupNameE)
                    Dim costingArray As Object() = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    Dim surchargeArray As Object() = {headerId, itemId, designId, itemNumber, priceGroup}

                    OrderCostings(costingArray)
                    CalculateSurcharge(surchargeArray)
                End If

                ' FIFTH BLIND
                If Not String.IsNullOrEmpty(priceProductGroupIdF) Then
                    itemNumber = 6

                    Dim sellArray As Object() = {priceProductGroupIdF, priceGroup, dropF, widthF, "Sell"}
                    Dim buyArray As Object() = {priceProductGroupIdF, priceGroup, dropF, widthF, "Buy"}

                    Dim dataPriceSell As DataRow = GetGridPrice(sellArray)
                    Dim dataPriceBuy As DataRow = GetGridPrice(buyArray)

                    Dim gridSellPrice As Decimal = 0D
                    Dim gridBuyPrice As Decimal = 0D

                    Dim gridSellMethod As String = String.Empty
                    Dim gridBuyMethod As String = String.Empty

                    Dim gridSellConditional As String = String.Empty
                    Dim gridBuyConditional As String = String.Empty

                    If Not dataPriceSell Is Nothing Then
                        gridSellMethod = dataPriceSell("Method").ToString()
                        gridSellPrice = dataPriceSell("Price")
                        gridSellConditional = dataPriceSell("Conditional").ToString()
                    End If
                    If Not dataPriceBuy Is Nothing Then
                        gridBuyMethod = dataPriceBuy("Method").ToString()
                        gridBuyPrice = dataPriceBuy("Price")
                        gridBuyConditional = dataPriceBuy("Conditional").ToString()
                    End If

                    Dim costSell As Decimal = gridSellPrice
                    Dim costBuy As Decimal = gridBuyPrice

                    thisSell = costSell
                    thisBuy = costBuy

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END ASC")
                    For Each discountRow As DataRow In discountData.Rows
                        Dim discountType As String = discountRow("Type").ToString()
                        Dim dataId As String = discountRow("DataId").ToString()
                        Dim discount As Decimal = CDec(discountRow("Discount"))

                        If (discountType = "Designs" AndAlso dataId <> designId) OrElse (discountType = "PriceProductGroups" AndAlso dataId <> priceProductGroupIdF) Then
                            Continue For
                        End If

                        Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                        thisSell = Math.Round(costSell - discountValue, 2)
                        If (designName = "Roller Blind" OrElse priceProductGroupNameF.Contains("Gear Reduction")) Then
                            discountValue = Math.Round((costSell - 7D) * discount / 100D, 2)

                            If isWithoutGR Then
                                thisSell = Math.Round(costSell - 7D - discountValue, 2)
                            ElseIf gridSellConditional = "Excl. $7 Disc" Then
                                thisSell = Math.Round((costSell - 7D) - discountValue + 7D, 2)
                            End If
                        End If

                        costSell = thisSell
                    Next

                    If gridSellMethod = "Square Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            thisSell = Math.Round(thisSell * squareMetreE, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            If squareMetreE < 1 Then thisSell = thisSell * 1
                            If squareMetreE >= 1 Then thisSell = Math.Round(thisSell * squareMetreE, 2)
                        End If
                        thisBuy = Math.Round(thisBuy * squareMetreE, 2)
                    End If
                    If gridSellMethod = "Linear Metre" Then
                        If companyDetailId = "2" OrElse companyDetailId = "3" OrElse companyDetailId = "4" OrElse companyDetailId = "8" Then
                            thisSell = Math.Round(thisSell * linearMetreE, 2)
                        End If
                        If companyDetailId = "5" OrElse companyDetailId = "6" Then
                            If linearMetreE < 1 Then thisSell = thisSell * 1
                            If linearMetreE >= 1 Then thisSell = Math.Round(thisSell * linearMetreE, 2)
                        End If
                        thisBuy = Math.Round(thisBuy * linearMetreE, 2)
                    End If

                    Dim buyPromoData As DataTable = GetDataTable("SELECT Id FROM Promos WHERE Active=1 AND Type='Buy' AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each buyPromoRow As DataRow In buyPromoData.Rows
                        Dim promoId As String = buyPromoRow("Id").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim isMatch As Boolean = False

                            Select Case promoType
                                Case "Designs"
                                    isMatch = (dataId = designId)
                                Case "Blinds"
                                    isMatch = (dataId = blindId)
                                Case "Products"
                                    isMatch = (dataId = productId)
                                Case "Fabrics"
                                    isMatch = (designName = "Roller Blind" AndAlso dataId = fabricIdF)
                                Case "FabricColours"
                                    isMatch = (designName = "Roller Blind" AndAlso dataId = fabricColourIdF)
                            End Select

                            If Not isMatch Then Continue For

                            Dim promoValue As Decimal = Math.Round(costBuy * discount / 100D, 2)
                            thisBuy = Math.Round(costBuy - promoValue, 2)
                            costBuy = thisBuy
                        Next
                    Next

                    Dim sellPromoData As DataTable = GetDataTable("SELECT CustomerPromos.PromoId FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    For Each sellPromoRow As DataRow In sellPromoData.Rows
                        Dim promoId As String = sellPromoRow("PromoId").ToString()
                        Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

                        For Each detailRow As DataRow In promoDetailData.Rows
                            Dim promoType As String = detailRow("Type").ToString()
                            Dim dataId As String = detailRow("DataId").ToString()
                            Dim discount As Decimal = CDec(detailRow("Discount"))

                            Dim compareId As String = ""

                            Select Case promoType
                                Case "Designs"
                                    compareId = designId
                                Case "Blinds"
                                    compareId = blindId
                                Case "Products"
                                    compareId = productId
                                Case "Fabrics"
                                    If designName = "Roller Blind" Then compareId = fabricIdF
                                Case "FabricColours"
                                    If designName = "Roller Blind" Then compareId = fabricColourIdF
                            End Select

                            If compareId <> dataId Then Continue For

                            Dim discountValue As Decimal = Math.Round(costSell * discount / 100D, 2)
                            thisSell = Math.Round(costSell - discountValue, 2)
                            costSell = thisSell
                        Next
                    Next

                    Dim costingDescription As String = String.Format("#6 {0}", priceProductGroupNameF)
                    Dim costingArray As Object() = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    Dim surchargeArray As Object() = {headerId, itemId, designId, itemNumber, priceGroup}

                    OrderCostings(costingArray)
                    CalculateSurcharge(surchargeArray)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub CalculateSurcharge(data As Object())
        Try
            Dim headerId As String = Convert.ToString(data(0))
            Dim itemId As String = Convert.ToString(data(1))
            Dim designId As String = Convert.ToString(data(2))
            Dim blindNo As String = Convert.ToString(data(3))
            Dim priceGroupId As String = Convert.ToString(data(4))

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@ItemId", If(String.IsNullOrWhiteSpace(itemId), CType(DBNull.Value, Object), CInt(itemId))),
                New SqlParameter("@DesignId", If(String.IsNullOrWhiteSpace(designId), CType(DBNull.Value, Object), CInt(designId))),
                New SqlParameter("@BlindNo", blindNo),
                New SqlParameter("@PriceGroupId", If(String.IsNullOrWhiteSpace(priceGroupId), CType(DBNull.Value, Object), CInt(priceGroupId)))
            }
            Dim surchargeData As DataTable = GetDataTableSP("sp_OrderDetails_Surcharge_Get", params)

            If surchargeData.Rows.Count > 0 Then
                For i As Integer = 0 To surchargeData.Rows.Count - 1
                    Dim name As String = surchargeData.Rows(i)("Name").ToString()
                    Dim buyCharge As String = surchargeData.Rows(i)("BuyCharge").ToString()
                    Dim sellCharge As String = surchargeData.Rows(i)("SellCharge").ToString()

                    Dim dataCosting As Object() = {headerId, itemId, blindNo, "Surcharge", name, buyCharge, sellCharge}
                    OrderCostings(dataCosting)
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub OrderCostings(data As Object())
        Try
            If data.Length = 7 Then
                Dim headerId As String = Convert.ToString(data(0))
                Dim itemId As String = Convert.ToString(data(1))
                Dim number As Integer = Convert.ToInt32(data(2))
                Dim type As String = Convert.ToString(data(3))
                Dim desc As String = Convert.ToString(data(4))
                Dim buyPrice As Decimal = Convert.ToDecimal(data(5))
                Dim sellPrice As Decimal = Convert.ToDecimal(data(6))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO OrderCostings VALUES(NEWID(), @HeaderId, @ItemId, @Number, @Type, @Description, @BuyPrice, @SellPrice)", thisConn)
                        thisCmd.Parameters.AddWithValue("@HeaderId", headerId)
                        thisCmd.Parameters.AddWithValue("@ItemId", itemId)
                        thisCmd.Parameters.AddWithValue("@Number", number)
                        thisCmd.Parameters.AddWithValue("@Type", type)
                        thisCmd.Parameters.AddWithValue("@Description", desc)
                        thisCmd.Parameters.AddWithValue("@BuyPrice", buyPrice)
                        thisCmd.Parameters.AddWithValue("@SellPrice", sellPrice)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub OrderCostings_Note(data As Object())
        Try
            If data.Length = 7 Then
                Dim headerId As String = Convert.ToString(data(0))
                Dim itemId As String = Convert.ToString(data(1))
                Dim number As Integer = Convert.ToInt32(data(2))
                Dim type As String = Convert.ToString(data(3))
                Dim desc As String = Convert.ToString(data(4))
                Dim buyPrice As Decimal = Convert.ToDecimal(data(5))
                Dim sellPrice As Decimal = Convert.ToDecimal(data(6))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM OrderCostings WHERE HeaderId=@HeaderId AND ItemId=@ItemId AND Type=@Type; INSERT INTO OrderCostings VALUES(NEWID(), @HeaderId, @ItemId, @Number, @Type, @Description, @BuyPrice, @SellPrice)", thisConn)
                        thisCmd.Parameters.AddWithValue("@HeaderId", headerId)
                        thisCmd.Parameters.AddWithValue("@ItemId", itemId)
                        thisCmd.Parameters.AddWithValue("@Number", number)
                        thisCmd.Parameters.AddWithValue("@Type", type)
                        thisCmd.Parameters.AddWithValue("@Description", desc)
                        thisCmd.Parameters.AddWithValue("@BuyPrice", buyPrice)
                        thisCmd.Parameters.AddWithValue("@SellPrice", sellPrice)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Logs(data As Object())
        Try
            If data.Length = 4 Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("sp_Logs_Insert", thisConn)
                        thisCmd.CommandType = CommandType.StoredProcedure
                        thisCmd.Parameters.AddWithValue("@Type", Convert.ToString(data(0)))
                        thisCmd.Parameters.AddWithValue("@DataId", Convert.ToString(data(1)))
                        thisCmd.Parameters.AddWithValue("@ActionBy", Convert.ToString(data(2)))
                        thisCmd.Parameters.AddWithValue("@Description", Convert.ToString(data(3)))
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub DeleteFilePrinting(orderId As String, fileName As String)
        Try
            Dim folderPath As String = HttpContext.Current.Server.MapPath(String.Format("~/File/Order/{0}", orderId))
            Dim filePath As String = IO.Path.Combine(folderPath, fileName)

            If IO.File.Exists(filePath) Then
                IO.File.Delete(filePath)
            End If
        Catch ex As Exception
        End Try
    End Sub

    ' REWORK
    Public Function IsReworkOrder(orderId As String) As Boolean
        Dim result As Boolean = False
        Try
            If Not String.IsNullOrEmpty(orderId) Then
                Using thisConn As New SqlConnection(myConn)
                    Dim query As String = "SELECT COUNT(*) FROM OrderReworks WHERE HeaderId=@HeaderId AND (Status='Unsubmitted' OR Status='Pending Approval' OR Status='Approved') AND Active=1"
                    Using thisCmd As New SqlCommand(query, thisConn)
                        thisCmd.Parameters.AddWithValue("@HeaderId", orderId)
                        thisConn.Open()
                        Dim count As Integer = Convert.ToInt32(thisCmd.ExecuteScalar())
                        result = (count > 0)
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function


    ' SHUTTER
    Public Function CountMultiLayout(input As String, substrings As String()) As Integer
        Dim count As Integer = 0
        Try
            For Each substring In substrings
                count += input.Split(substring).Length - 1
            Next
        Catch ex As Exception
            count = 0
        End Try
        Return count
    End Function

    Public Function CheckStringLayoutD(layout As String) As Boolean
        Try
            If String.IsNullOrEmpty(layout) Then
                Return False
            End If

            For i As Integer = 0 To layout.Length - 1
                If layout(i) = "D"c Then
                    Dim hasDashBefore As Boolean = (i > 0 AndAlso layout(i - 1) = "-"c)
                    Dim hasDashAfter As Boolean = (i < layout.Length - 1 AndAlso layout(i + 1) = "-"c)

                    If Not (hasDashBefore Or hasDashAfter) Then
                        Return False
                    End If
                End If
            Next

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function GetPanelQty(data As String()) As Integer
        Dim result As Integer = 0

        Dim blindName As String = data(0)
        Dim panelQty As String = data(1)
        Dim layout As String = data(2)
        Dim horizontalHeight As String = data(3)

        Dim countL As Integer = 0
        Dim countR As Integer = 0
        Dim countF As Integer = 0
        Dim countM As Integer = 0
        Dim countB As Integer = 0

        countL = layout.Split("L").Length - 1
        countR = layout.Split("R").Length - 1
        countF = layout.Split("F").Length - 1
        countM = layout.Split("M").Length - 1
        If blindName = "Track Sliding" Then
            countB = layout.Split("B").Length - 1
        End If

        Dim hitung As Integer = countL + countR + countF + countM + countB
        If horizontalHeight > 0 Then
            hitung = (countL + countR + countF + countM + countB) * 2
        End If

        If blindName = "Panel Only" Then
            hitung = panelQty
        End If

        result = hitung

        Return result
    End Function

    Public Function WidthDeductShutter(data As Object()) As Decimal
        Dim result As Decimal = 0.00

        Dim blindName As String = Convert.ToString(data(0))
        Dim type As String = Convert.ToString(data(1))
        Dim width As Integer = Convert.ToInt32(data(2))

        If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim hingeDeduction As Decimal = 2.5
            Dim frameDeduction As Decimal = 0
            Dim frameLDeduction As Decimal = 0
            Dim frameRDeduction As Decimal = 0
            Dim tPostDeduction As Decimal = 0
            Dim bPostDeduction As Decimal = 0
            Dim cPostDeduction As Decimal = 0
            Dim csDeduction As Decimal = 0
            Dim crDeduction As Decimal = 0
            Dim ccDeduction As Decimal = 0

            Dim jumlahT As Integer = layoutCode.Split("T").Length - 1
            Dim jumlahB As Integer = layoutCode.Split("B").Length - 1
            Dim jumlahC As Integer = layoutCode.Split("C").Length - 1
            Dim jumlahD As Integer = layoutCode.Split("D").Length - 1

            If mounting = "Inside" Then
                crDeduction = -2
                If frameType.Contains("Z Frame") Then
                    crDeduction = -3.2
                End If
            End If

            If frameType = "Beaded L 48mm" Then frameDeduction = 25.4
            If frameType = "Insert L 50mm" Then frameDeduction = 22.2
            If frameType = "Insert L 63mm" Then frameDeduction = 22.2
            If frameType = "Small Bullnose Z Frame" Then frameDeduction = 19.6
            If frameType = "Large Bullnose Z Frame" Then frameDeduction = 22.7
            If frameType = "Colonial Z Frame" Then frameDeduction = 18.4

            If Not frameLeft = "No" Then
                frameLDeduction = frameDeduction
            End If
            If Not frameRight = "No" Then
                frameRDeduction = frameDeduction
            End If

            If layoutCode.Contains("D") Then csDeduction = 3.5 / 2
            If layoutCode.Contains("T") Then tPostDeduction = 25.4 / 2

            If layoutCode.Contains("B") Then
                If frameType = "Beaded L 48mm" Or frameType = "Insert L 50mm" Or frameType = "Insert L 63mm" Or frameType = "No Frame" Then
                    bPostDeduction = 30 / 2
                End If
                If mounting = "Inside" And frameType = "Small Bullnose Z Frame" Then
                    bPostDeduction = 38 / 2
                End If
                If mounting = "Inside" And frameType = "Large Bullnose Z Frame" Then
                    bPostDeduction = 40 / 2
                End If
                If mounting = "Inside" And frameType = "Colonial Z Frame" Then
                    bPostDeduction = 34 / 2
                End If
                If mounting = "Outside" And frameType = "Beaded L 48mm" Then
                    bPostDeduction = 34.7 / 2
                End If
                If mounting = "Outside" And frameType = "Insert L 50mm" Then
                    bPostDeduction = 34.7 / 2
                End If
                If mounting = "Outside" And frameType = "Insert L 63mm" Then
                    bPostDeduction = 40.4 / 2
                End If
            End If

            If layoutCode.Contains("C") Then
                If mounting = "Inside" Then
                    If frameType = "Beaded L 48mm" Or frameType = "Insert L 50mm" Or frameType = "Insert L 63mm" Or frameType = "No Frame" Then cPostDeduction = 30 / 2
                    If frameType = "Small Bullnose Z Frame" Then cPostDeduction = 42 / 2
                    If frameType = "Large Bullnose Z Frame" Then cPostDeduction = 44 / 2
                    If frameType = "Colonial Z Frame" Then cPostDeduction = 36 / 2
                End If
                If mounting = "Outside" Then
                    If frameType = "Beaded L 48mm" Then cPostDeduction = 62.5 / 2
                    If frameType = "Insert L 50mm" Then cPostDeduction = 65 / 2
                    If frameType = "Insert L 63mm" Then cPostDeduction = 78 / 2
                End If
            End If

            If type = "All" Then
                result = width - (hingeDeduction * panelQty) - ((frameLDeduction + frameRDeduction) * panelQty) - (tPostDeduction * jumlahT) - (bPostDeduction * jumlahB) - (cPostDeduction * jumlahC) - (csDeduction * jumlahD) - (crDeduction * panelQty) - (ccDeduction * panelQty)
            End If

            If type = "Gap" Then
                result = width - (hingeDeduction * panelQty) - (frameLDeduction + frameRDeduction) - tPostDeduction - bPostDeduction - cPostDeduction - csDeduction - crDeduction - ccDeduction
            End If
        End If

        If blindName = "Track Bi-fold" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim insideDeductions As Decimal = 0.00
            Dim pivotDeductions As Decimal = 0.00
            Dim closingDeductions As Decimal = 5
            Dim frameLDeductions As Decimal = 0.00
            Dim frameRDeductions As Decimal = 0.00
            Dim hingedDeductions As Decimal = 0.00

            Dim result1 As Integer = 0
            Dim parts As String() = layoutCode.Split("/"c)
            If parts.Length > 0 Then
                result1 = CountMultiLayout(parts(0), New String() {"L", "R", "F"}) - 1
            End If

            Dim result2 As Integer = 0
            If layoutCode.Contains("/") Then
                Dim partss As String() = layoutCode.Split("/"c)
                If partss.Length > 1 Then
                    result2 = CountMultiLayout(partss(1), New String() {"L", "R", "F"}) - 1
                End If
            End If

            hingedDeductions = (result1 + result2) * 5

            If mounting = "Inside" Then insideDeductions = 2

            Dim uniqueLetters As New HashSet(Of Char)
            For Each c As Char In layoutCode.ToLower()
                If Char.IsLetter(c) Then
                    uniqueLetters.Add(c)
                End If
            Next

            pivotDeductions = 5 * uniqueLetters.Count

            If Not frameLeft = "No" Then frameLDeductions = 19
            If Not frameRight = "No" Then frameRDeductions = 19

            result = width - insideDeductions - pivotDeductions - closingDeductions - frameLDeductions - frameRDeductions - hingedDeductions
        End If

        If blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            Dim frameLDeductions As Decimal = 0
            Dim frameRDeductions As Decimal = 0
            Dim insideDeductions As Decimal = 0

            If Not frameLeft = "No" Then frameLDeductions = 19 * panelQty
            If Not frameRight = "No" Then frameRDeductions = 19 * panelQty
            If mounting = "Inside" Then insideDeductions = 2

            result = width - frameLDeductions - frameRDeductions - insideDeductions
        End If

        If blindName = "Fixed" Then
            Dim mounting As String = Convert.ToString(data(3))
            Dim layoutCode As String = Convert.ToString(data(4))
            Dim frameType As String = Convert.ToString(data(5))
            Dim frameLeft As String = Convert.ToString(data(6))
            Dim frameRight As String = Convert.ToString(data(7))
            Dim panelQty As Integer = Convert.ToInt32(data(8))

            If frameType = "U Channel" Then
                Dim clearanceLDeduction As Decimal = 1
                Dim clearanceRDeduction As Decimal = 1

                Dim frameLDeduction As Decimal = 0
                Dim frameRDeduction As Decimal = 0


                If frameLeft = "L Strip" Then clearanceLDeduction = 2
                If frameRight = "L Strip" Then clearanceRDeduction = 2

                If frameLeft = "L Strip" Then frameLDeduction = 7.25
                If frameRight = "L Strip" Then frameRDeduction = 7.25

                result = width - clearanceLDeduction - clearanceRDeduction - frameLDeduction - frameRDeduction
            End If

            If frameType = "19x19 Light Block" Then
                Dim clearanceDeduction As Decimal = 5

                result = width - clearanceDeduction
            End If
        End If
        Return result
    End Function

    Public Function HeightDeductShutter(data As Object()) As Decimal
        Dim result As Decimal = 0

        Dim blindName As String = Convert.ToString(data(0))
        Dim drop As Integer = Convert.ToInt32(data(1))
        Dim mounting As String = Convert.ToString(data(2))
        Dim frameType As String = Convert.ToString(data(3))
        Dim frameTop As String = Convert.ToString(data(4))
        Dim frameBottom As String = Convert.ToString(data(5))
        Dim bottomTrackType As String = Convert.ToString(data(6))
        Dim horizontalTPost As String = Convert.ToString(data(7))

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            Dim crDeductionTop As Decimal = 0
            Dim crDeductionBottom As Decimal = 0

            Dim cfDeductionTop As Decimal = 0
            Dim cfDeductionBottom As Decimal = 0

            Dim frameDeduction As Decimal = 0
            Dim frameTDeduction As Decimal = 0
            Dim frameBDeduction As Decimal = 0
            Dim postDeduction As Decimal = 0

            If mounting = "Inside" Then
                If frameType = "Beaded L 48mm" Or frameType = "Insert L 50mm" Or frameType = "Insert L 63mm" Or frameType = "Flat L 48mm" Then
                    If frameTop = "L Striker Plate" Then crDeductionTop = 2
                    If frameBottom = "L Striker Plate" Then crDeductionBottom = 2
                End If

                If frameType.Contains("Z Frame") Then
                    If frameTop = "L Striker Plate" Then crDeductionTop = 3.2
                    If frameBottom = "L Striker Plate" Then crDeductionBottom = 3.2
                End If
            End If

            If Not frameTop = "No" Then cfDeductionTop = 3
            If Not frameBottom = "No" Then cfDeductionBottom = 3

            If frameType = "Beaded L 48mm" Then frameDeduction = 25.4
            If frameType = "Insert L 50mm" Then frameDeduction = 22.2
            If frameType = "Insert L 63mm" Then frameDeduction = 22.2
            If frameType = "Small Bullnose Z Frame" Then frameDeduction = 19.6
            If frameType = "Large Bullnose Z Frame" Then frameDeduction = 22.7
            If frameType = "Colonial Z Frame" Then frameDeduction = 18.4

            If Not frameTop = "No" Then frameTDeduction = frameDeduction
            If Not frameBottom = "No" Then frameBDeduction = frameDeduction

            If frameTop = "L Striker Plate" Or frameTop.Contains("Sill Plate") Then
                frameTDeduction = frameDeduction + 9.5
            End If

            If frameBottom = "L Striker Plate" Or frameBottom.Contains("Sill Plate") Then
                frameBDeduction = frameDeduction + 9.5
            End If

            If horizontalTPost = "No Post" Then postDeduction = 3 / 2
            If horizontalTPost = "Yes" Then postDeduction = 25.4 / 2

            result = drop - crDeductionTop - crDeductionBottom - cfDeductionTop - cfDeductionBottom - frameTDeduction - frameBDeduction - postDeduction
        End If

        If blindName = "Track Bi-fold" Or blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            Dim crTopDeduction As Decimal = 0
            Dim crBottomDeduction As Decimal = 0
            Dim trDeduction As Decimal = 51
            Dim mtDeduction As Decimal = 0
            Dim utDeduction As Decimal = 0
            Dim utfDeduction As Decimal = 0
            Dim frameDeduction As Decimal = 0
            Dim frameTDeduction As Decimal = 0
            Dim frameBDeduction As Decimal = 0

            If mounting = "Inside" Then crTopDeduction = 1
            If bottomTrackType = "M Track" Then mtDeduction = 24
            If bottomTrackType = "U Track" Then utDeduction = 33

            If frameTop = "Yes" Then frameTDeduction = 1

            If frameType = "100mm" Then frameDeduction = 19
            If frameType = "160mm" Then frameDeduction = 19
            If frameType = "200mm" Then frameDeduction = 19

            If frameTop = "Yes" Then frameTDeduction = frameDeduction

            result = drop - crTopDeduction - crBottomDeduction - trDeduction - mtDeduction - utDeduction - utfDeduction - frameTDeduction - frameBDeduction
        End If

        If blindName = "Fixed" Then
            If frameType = "U Channel" Then
                Dim topDeduction As Decimal = 0
                Dim bottomDeduction As Decimal = 0

                If frameTop = "Yes" Then topDeduction = 17.5
                If frameBottom = "Yes" Then bottomDeduction = 17.5

                result = drop - topDeduction - bottomDeduction
            End If

            If frameType = "19x19 Light Block" Then
                result = drop - 6
            End If
        End If
        Return result
    End Function
End Class
