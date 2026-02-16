Imports System.Data
Imports System.Data.SqlClient

Public Class OrderClass
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

    Public Function GetDataRowSP(spName As String, params As List(Of SqlParameter)) As DataRow
        Try
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand(spName, conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddRange(params.ToArray())

                    Using da As New SqlDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Return dt.Rows(0)
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

    Public Function GetDataTableSP(spName As String, params As List(Of SqlParameter)) As DataTable
        Dim dt As New DataTable()
        Using conn As New SqlConnection(myConn)
            Using cmd As New SqlCommand(spName, conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddRange(params.ToArray())

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Return dt
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
                    Using myCmd As New SqlCommand("SELECT QueryString FROM OrderActionContext WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", id)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Email FROM CustomerContacts WHERE CustomerId=@CustomerId AND [Primary]=1", thisConn)
                        myCmd.Parameters.AddWithValue("@CustomerId", customerId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT LoginRoles.Name FROM CustomerLogins INNER JOIN LoginRoles ON CustomerLogins.RoleId=LoginRoles.Id WHERE CustomerLogins.Id=@LoginId", thisConn)
                        myCmd.Parameters.AddWithValue("@LoginId", loginId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT CASE WHEN Pricing=1 THEN 'Yes' ELSE '' END AS PriceAccess FROM CustomerLogins WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", loginId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT CustomerId FROM OrderHeaders WHERE Id=@OrderId", thisConn)
                        myCmd.Parameters.AddWithValue("@OrderId", headerId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Customers.CompanyId FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id=@OrderId", thisConn)
                        myCmd.Parameters.AddWithValue("@OrderId", orderId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Customers.CompanyDetailId FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id=@OrderId", thisConn)
                        myCmd.Parameters.AddWithValue("@OrderId", orderId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Companys.Alias FROM Customers LEFT JOIN Companys ON Customers.CompanyId=Companys.Id WHERE Customers.Id=@CustomerId", thisConn)
                        myCmd.Parameters.AddWithValue("@CustomerId", customerId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Name FROM CompanyDetails WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", companyDetailId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT CompanyDetailId FROM Customers WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", customerId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT CompanyDetails.Name FROM Customers LEFT JOIN CompanyDetails ON Customers.CompanyDetailId=CompanyDetails.Id WHERE Customers.Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", customerId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Customers.PriceGroupId FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id=@OrderId", thisConn)
                        myCmd.Parameters.AddWithValue("@OrderId", orderId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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

    Public Function GetPriceProductGroupId(groupName As String, designId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(groupName) OrElse String.IsNullOrEmpty(designId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("SELECT Id FROM PriceProductGroups WHERE Name=@Name AND DesignId=@DesignId AND Active=1", thisConn)
                        myCmd.Parameters.AddWithValue("@Name", groupName)
                        myCmd.Parameters.AddWithValue("@DesignId", designId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Name FROM PriceProductGroups WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", groupId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT DesignId FROM Products WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", productId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT BlindId FROM Products WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", productId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT TubeType FROM Products WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", productId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT ControlType FROM Products WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", productId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Name FROM Designs WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", designId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Page FROM Designs WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", designId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Name FROM Blinds WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", blindId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Name FROM Products WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", productId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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

    Public Function GetTubeName(tubeId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(tubeId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("SELECT Name FROM ProductTubes WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", tubeId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Name FROM ProductControls WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", controlId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Name FROM ProductColours WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", colourId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Name FROM Fabrics WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", fabricId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Name FROM FabricColours WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", fabricColourId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT [Group] FROM Fabrics WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", fabricId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                        Using myCmd As New SqlCommand(thisQuery, thisConn)
                            myCmd.Parameters.AddWithValue("@Id", fabricId)

                            thisConn.Open()
                            Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Factory FROM FabricColours WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", fabricColourId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Name FROM Chains WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", chainId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT ChainType FROM Chains WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", chainId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT Name FROM Bottoms WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", bottomId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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
            Dim result As New System.Text.StringBuilder()

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
                Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderHeaders ORDER BY Id DESC", thisConn)

                    thisConn.Open()

                    Dim lastId As Object = myCmd.ExecuteScalar()
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
                Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderDetails ORDER BY Id DESC", thisConn)

                    thisConn.Open()

                    Dim lastId As Object = myCmd.ExecuteScalar()
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
                Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderReworks ORDER BY Id DESC", thisConn)

                    thisConn.Open()

                    Dim lastId As Object = myCmd.ExecuteScalar()
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
                Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderReworkDetails ORDER BY Id DESC", thisConn)

                    thisConn.Open()

                    Dim lastId As Object = myCmd.ExecuteScalar()
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

    Public Function IsOrderExist(customerId As String, orderNumber As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("SELECT OrderNumber FROM OrderHeaders WHERE CustomerId=@CustomerId AND OrderNumber=@OrderNumber AND Active=1", thisConn)
                    myCmd.Parameters.AddWithValue("@CustomerId", customerId)
                    myCmd.Parameters.AddWithValue("@OrderNumber", orderNumber)

                    thisConn.Open()
                    Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT CashSale FROM Customers WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", customerId)
                        thisConn.Open()

                        Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT OnStop FROM Customers WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", customerId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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

    Public Function GetTotalItemOrder(headerId As String) As Integer
        Dim result As String = 0
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("SELECT SUM (CASE WHEN Designs.Type='Blinds' THEN OrderDetails.TotalItems WHEN Designs.Type='Shutters' THEN 1 ELSE 0 END) AS totalOrder FROM OrderHeaders INNER JOIN OrderDetails ON OrderHeaders.Id=OrderDetails.HeaderId INNER JOIN Products ON OrderDetails.ProductId=Products.Id INNER JOIN Designs ON Products.DesignId=Designs.Id WHERE OrderHeaders.Id=@Id AND OrderHeaders.Active=1 AND OrderDetails.Active=1 AND Designs.Type IN ('Blinds', 'Shutters');", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", headerId)

                    thisConn.Open()
                    Dim obj = myCmd.ExecuteScalar()
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
                    Using myCmd As New SqlCommand("SELECT '<b>' + CustomerLogins.FullName + '</b> on ' + FORMAT(Logs.ActionDate, 'dd MMMM yyyy HH:mm') + '. Action : ' + Logs.Description AS FinalLog FROM Logs LEFT JOIN CustomerLogins ON Logs.ActionBy = CustomerLogins.Id WHERE Logs.Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", logId)
                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
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


    ' PRICING
    Public Function GetGridPrice(data As Object()) As Decimal
        Dim result As Decimal = 0D
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

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(String.Format("SELECT TOP 1 Price FROM PriceBases WHERE ProductGroupId='{0}' AND PriceGroupId='{1}' AND Height>={2} AND Width>={3} AND Category='{4}' AND Price>=0 ORDER BY Height, Width, Price ASC", productGroupId, priceGroupId, drop, width, category), thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        If rdResult.HasRows Then
                            While rdResult.Read
                                result = rdResult.Item(0)
                            End While
                        End If
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = 1
        End Try
        Return result
    End Function

    Public Function GetGridMethod(data As Object()) As String
        Dim result As String = String.Empty
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

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(String.Format("SELECT TOP 1 Method FROM PriceBases WHERE ProductGroupId='{0}' AND PriceGroupId='{1}' AND Height>={2} AND Width>={3} AND Category='{4}' AND Price>=0 ORDER BY Height, Width, Price ASC", productGroupId, priceGroupId, drop, width, category), thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        If rdResult.HasRows Then
                            While rdResult.Read
                                result = rdResult.Item(0).ToString()
                            End While
                        Else
                            result = String.Empty
                        End If
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Sub ResetPriceDetail(headerId As String, itemId As String)
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM OrderCostings WHERE HeaderId=@HeaderId AND ItemId=@ItemId", thisConn)
                    myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                    myCmd.Parameters.AddWithValue("@ItemId", itemId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub FinalCostItem(headerId As String, itemId As String)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM OrderCostings WHERE ItemId=@ItemId AND Type='Final'", thisConn)
                    myCmd.Parameters.AddWithValue("@ItemId", itemId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
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
            Dim thisData As DataTable = GetDataTable("SELECT * FROM OrderDetails WHERE HeaderId='" & headerId & "' AND Active=1")
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

                Dim linearMetre As Decimal = 0
                If Not IsDBNull(thisData("LinearMetre")) Then
                    linearMetre = Convert.ToDecimal(thisData("LinearMetre"))
                End If
                Dim linearMetreB As Decimal = 0
                If Not IsDBNull(thisData("LinearMetreB")) Then
                    linearMetreB = Convert.ToDecimal(thisData("LinearMetreB"))
                End If
                Dim linearMetreC As Decimal = 0
                If Not IsDBNull(thisData("LinearMetreC")) Then
                    linearMetreC = Convert.ToDecimal(thisData("LinearMetreC"))
                End If
                Dim linearMetreD As Decimal = 0
                If Not IsDBNull(thisData("LinearMetreD")) Then
                    linearMetreD = Convert.ToDecimal(thisData("LinearMetreD"))
                End If
                Dim linearMetreE As Decimal = 0
                If Not IsDBNull(thisData("LinearMetreE")) Then
                    linearMetreE = Convert.ToDecimal(thisData("LinearMetreE"))
                End If
                Dim linearMetreF As Decimal = 0
                If Not IsDBNull(thisData("LinearMetreF")) Then
                    linearMetreF = Convert.ToDecimal(thisData("LinearMetreF"))
                End If

                Dim squareMetre As Decimal = 0
                If Not IsDBNull(thisData("SquareMetre")) Then
                    squareMetre = Convert.ToDecimal(thisData("SquareMetre"))
                End If
                Dim squareMetreB As Decimal = 0
                If Not IsDBNull(thisData("SquareMetreB")) Then
                    squareMetreB = Convert.ToDecimal(thisData("SquareMetreB"))
                End If
                Dim squareMetreC As Decimal = 0
                If Not IsDBNull(thisData("SquareMetreC")) Then
                    squareMetreC = Convert.ToDecimal(thisData("SquareMetreC"))
                End If
                Dim squareMetreD As Decimal = 0
                If Not IsDBNull(thisData("SquareMetreD")) Then
                    squareMetreD = Convert.ToDecimal(thisData("SquareMetreD"))
                End If
                Dim squareMetreE As Decimal = 0
                If Not IsDBNull(thisData("SquareMetreE")) Then
                    squareMetreE = Convert.ToDecimal(thisData("SquareMetreE"))
                End If
                Dim squareMetreF As Decimal = 0
                If Not IsDBNull(thisData("SquareMetreF")) Then
                    squareMetreF = Convert.ToDecimal(thisData("SquareMetreF"))
                End If

                Dim objectArray As Object() = Nothing

                Dim itemNumber As Integer = 0

                Dim thisSell As Decimal = 0
                Dim thisSellAdditional As Decimal = 0
                Dim thisBuy As Decimal = 0
                Dim thisBuyAdditional As Decimal = 0

                If Not String.IsNullOrEmpty(priceProductGroupId) Then
                    itemNumber = 1

                    objectArray = {priceProductGroupId, priceGroup, drop, width, "Sell"}
                    If designName = "Skyline Shutter Express" OrElse designName = "Skyline Shutter Ocean" OrElse designName = "Evolve Shutter Express" OrElse designName = "Evolve Shutter Ocean" Then
                        objectArray = {priceProductGroupId, shutterPriceGroup, drop, width, "Sell"}
                    End If
                    If designName = "Window" OrElse designName = "Door" Then
                        objectArray = {priceProductGroupId, doorPriceGroup, drop, width, "Sell"}
                    End If

                    Dim gridType As String = GetGridMethod(objectArray)

                    Dim gridSell As Decimal = GetGridPrice(objectArray)
                    Dim gridSellAdditional As Decimal = 0

                    If Not String.IsNullOrEmpty(priceAdditional) Then
                        objectArray = {priceAdditional, priceGroup, 0, width, "Sell"}
                        gridSellAdditional = GetGridPrice(objectArray)
                    End If

                    objectArray = {priceProductGroupId, priceGroup, drop, width, "Buy"}
                    If designName = "Skyline Shutter Express" OrElse designName = "Skyline Shutter Ocean" OrElse designName = "Evolve Shutter Express" OrElse designName = "Evolve Shutter Ocean" Then
                        objectArray = {priceProductGroupId, shutterPriceGroup, drop, width, "Buy"}
                    End If
                    If designName = "Window" OrElse designName = "Door" Then
                        objectArray = {priceProductGroupId, doorPriceGroup, drop, width, "Buy"}
                    End If
                    Dim gridBuy As Decimal = GetGridPrice(objectArray)

                    Dim gridBuyAdditional As Decimal = 0
                    If Not String.IsNullOrEmpty(priceAdditional) Then
                        objectArray = {priceAdditional, priceGroup, 0, width, "Buy"}
                        gridBuyAdditional = GetGridPrice(objectArray)
                    End If

                    Dim costSell As Decimal = gridSell
                    Dim costSellAdditional As Decimal = gridSellAdditional

                    Dim costBuy As Decimal = gridBuy
                    Dim costBuyAdditional As Decimal = gridBuyAdditional

                    thisSell = costSell

                    thisSellAdditional = costSellAdditional

                    thisBuy = costBuy
                    thisBuyAdditional = costBuyAdditional

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
                    If discountData.Rows.Count > 0 Then
                        For i As Integer = 0 To discountData.Rows.Count - 1
                            Dim discountType As String = discountData.Rows(i)("Type").ToString()
                            Dim dataId As String = discountData.Rows(i)("DataId").ToString()
                            Dim discountValue As Decimal = discountData.Rows(i)("Discount")

                            If discountType = "Designs" Then
                                If dataId = designId Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If
                                    Dim additionalValue As Decimal = costSellAdditional * discountValue / 100

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If
                                    thisSellAdditional = gridSellAdditional - additionalValue

                                    costSell = thisSell
                                    costSellAdditional = thisSellAdditional
                                End If
                            End If

                            If discountType = "PriceProductGroups" Then
                                If dataId = priceProductGroupId Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If
                                    Dim additionalValue As Decimal = costSellAdditional * discountValue / 100

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If
                                    thisSellAdditional = gridSellAdditional - additionalValue

                                    costSell = thisSell
                                    costSellAdditional = thisSellAdditional
                                End If
                            End If
                        Next
                    End If

                    If gridType = "Square Metre" Then
                        If companyId = "3" Then
                            If squareMetre < 1 Then thisSell = thisSell * 1
                            If squareMetre >= 1 Then thisSell = thisSell * squareMetre
                        End If
                        If companyId = "2" OrElse companyId = "4" OrElse companyId = "5" Then
                            thisSell = thisSell * squareMetre
                        End If
                        thisBuy = thisBuy * squareMetre
                    End If

                    If gridType = "Linear Metre" Then
                        If companyId = "3" Then
                            If linearMetre < 1 Then thisSell = thisSell * 1
                            If linearMetre >= 1 Then thisSell = thisSell * linearMetre
                        End If
                        If companyId = "2" OrElse companyId = "4" OrElse companyId = "5" Then
                            thisSell = thisSell * linearMetre
                        End If
                        thisBuy = thisBuy * linearMetre
                    End If

                    If designName = "Skyline Shutter Express" OrElse designName = "Skyline Shutter Ocean" OrElse designName = "Evolve Shutter Express" OrElse designName = "Evolve Shutter Ocean" Then
                        If companyId = "2" OrElse companyId = "4" Then
                            If squareMetre <= 0.75 Then thisSell = gridSell * 0.75
                        End If
                    End If

                    Dim promoData As DataTable = GetDataTable("SELECT CustomerPromos.* FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    If promoData.Rows.Count > 0 Then
                        For i As Integer = 0 To promoData.Rows.Count - 1
                            Dim promoId As String = promoData.Rows(i)("PromoId").ToString()

                            Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")
                            If promoDetailData.Rows.Count > 0 Then
                                For iDetail As Integer = 0 To promoDetailData.Rows.Count - 1
                                    Dim promoType As String = promoDetailData.Rows(iDetail)("Type").ToString()
                                    Dim dataId As String = promoDetailData.Rows(iDetail)("DataId").ToString()
                                    Dim promoValue As Decimal = promoDetailData.Rows(iDetail)("Discount")

                                    If promoType = "Designs" Then
                                        If dataId = designId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Blinds" Then
                                        If dataId = blindId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Products" Then
                                        If dataId = productId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Fabrics" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "FabricColours" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricColourId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "FrameColours" AndAlso (designName = "Door" OrElse designName = "Window") Then
                                        If dataId = frameColour Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If

                    Dim costingDescription As String = priceProductGroupName
                    If totalItems > 1 AndAlso Not (designName = "Skyline Shutter Express" OrElse designName = "Skyline Shutter Ocean" OrElse designName = "Evolve Shutter Express" OrElse designName = "Evolve Shutter Ocean") Then
                        costingDescription = String.Format("#1 {0}", priceProductGroupName)
                    End If

                    If designName = "Door" AndAlso (tubeName = "Hinged Double" OrElse tubeName = "Sliding Double") Then
                        costingDescription = String.Format("#1 {0}", priceProductGroupName)
                    End If

                    objectArray = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    OrderCostings(objectArray)

                    If blindName = "Single Curtain & Track" OrElse blindName = "Double Curtain & Track" Then
                        costingDescription = String.Format("Track for Curtain ({0})", trackType)
                        If blindName = "Double Curtain & Track" Then
                            costingDescription = String.Format("#1 Track for Curtain ({0})", trackType)
                        End If
                        objectArray = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuyAdditional, thisSellAdditional}
                        OrderCostings(objectArray)
                    End If

                    objectArray = {headerId, itemId, itemNumber, priceGroup}
                    If designName = "Skyline Shutter Express" OrElse designName = "Skyline Shutter Ocean" OrElse designName = "Evolve Shutter Express" OrElse designName = "Evolve Shutter Ocean" Then
                        objectArray = {headerId, itemId, itemNumber, shutterPriceGroup}
                    End If
                    If designName = "Window" OrElse designName = "Door" Then
                        objectArray = {headerId, itemId, itemNumber, doorPriceGroup}
                    End If
                    CalculateCharge(objectArray)
                End If

                If Not String.IsNullOrEmpty(priceProductGroupIdB) Then
                    itemNumber = 2

                    objectArray = {priceProductGroupIdB, priceGroup, dropB, widthB, "Sell"}
                    If designName = "Door" Then
                        objectArray = {priceProductGroupIdB, doorPriceGroup, drop, width, "Sell"}
                    End If

                    Dim gridType As String = GetGridMethod(objectArray)
                    Dim gridSell As Decimal = GetGridPrice(objectArray)
                    Dim gridSellAdditional As Decimal = 0

                    If Not String.IsNullOrEmpty(priceAdditionalB) Then
                        objectArray = {priceAdditionalB, priceGroup, 0, widthB, "Sell"}
                        gridSellAdditional = GetGridPrice(objectArray)
                    End If

                    objectArray = {priceProductGroupIdB, priceGroup, dropB, widthB, "Buy"}
                    If designName = "Door" Then
                        objectArray = {priceProductGroupIdB, doorPriceGroup, drop, width, "Buy"}
                    End If
                    Dim gridBuy As Decimal = GetGridPrice(objectArray)
                    Dim gridBuyAdditional As Decimal = 0

                    If Not String.IsNullOrEmpty(priceAdditionalB) Then
                        objectArray = {priceAdditionalB, priceGroup, 0, widthB, "Buy"}
                        gridBuyAdditional = GetGridPrice(objectArray)
                    End If

                    Dim costSell As Decimal = gridSell
                    Dim costSellAdditional As Decimal = gridSellAdditional

                    Dim costBuy As Decimal = gridBuy
                    Dim costBuyAdditional As Decimal = gridBuyAdditional

                    thisSell = costSell
                    thisSellAdditional = costSellAdditional

                    thisBuy = costBuy
                    thisBuyAdditional = costBuyAdditional

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
                    If discountData.Rows.Count > 0 Then
                        For i As Integer = 0 To discountData.Rows.Count - 1
                            Dim discountType As String = discountData.Rows(i)("Type").ToString()
                            Dim dataId As String = discountData.Rows(i)("DataId").ToString()
                            Dim discountValue As Decimal = discountData.Rows(i)("Discount")

                            If discountType = "Designs" Then
                                If dataId = designId Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If

                                    Dim additionalValue As Decimal = costSellAdditional * discountValue / 100

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If
                                    thisSellAdditional = gridSellAdditional - additionalValue

                                    costSell = thisSell
                                    costSellAdditional = thisSellAdditional
                                End If
                            End If

                            If discountType = "PriceProductGroups" Then
                                If dataId = priceProductGroupIdB Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If
                                    Dim additionalValue As Decimal = costSellAdditional * discountValue / 100

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If
                                    thisSellAdditional = gridSellAdditional - additionalValue

                                    costSell = thisSell
                                    costSellAdditional = thisSellAdditional
                                End If
                            End If
                        Next
                    End If

                    If gridType = "Square Metre" Then
                        If companyId = "3" Then
                            If squareMetre < 1 Then thisSell = thisSell * 1
                            If squareMetre >= 1 Then thisSell = thisSell * squareMetreB
                        End If
                        If companyId = "2" OrElse companyId = "4" OrElse companyId = "5" Then
                            thisSell = thisSell * squareMetreB
                        End If
                        thisBuy = thisBuy * squareMetreB
                    End If
                    If gridType = "Linear Metre" Then
                        If companyId = "3" Then
                            If linearMetre < 1 Then thisSell = thisSell * 1
                            If linearMetre >= 1 Then thisSell = thisSell * linearMetreB
                        End If
                        If companyId = "2" OrElse companyId = "4" OrElse companyId = "5" Then
                            thisSell = thisSell * linearMetreB
                        End If
                        thisBuy = thisBuy * linearMetreB
                    End If

                    Dim promoData As DataTable = GetDataTable("SELECT CustomerPromos.* FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    If promoData.Rows.Count > 0 Then
                        For i As Integer = 0 To promoData.Rows.Count - 1
                            Dim promoId As String = promoData.Rows(i)("PromoId").ToString()

                            Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")
                            If promoDetailData.Rows.Count > 0 Then
                                For iDetail As Integer = 0 To promoDetailData.Rows.Count - 1
                                    Dim promoType As String = promoDetailData.Rows(iDetail)("Type").ToString()
                                    Dim dataId As String = promoDetailData.Rows(iDetail)("DataId").ToString()
                                    Dim promoValue As Decimal = promoDetailData.Rows(iDetail)("Discount")

                                    If promoType = "Designs" Then
                                        If dataId = designId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Blinds" Then
                                        If dataId = blindId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Products" Then
                                        If dataId = productId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Fabrics" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricIdB Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "FabricColours" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricColourIdB Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If

                    Dim costingDescription As String = String.Format("#2 {0}", priceProductGroupNameB)

                    objectArray = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    OrderCostings(objectArray)

                    If blindName = "Double Curtain & Track" Then
                        costingDescription = String.Format("#2 Track for Curtain ({0})", trackTypeb)
                        objectArray = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuyAdditional, thisSellAdditional}
                        OrderCostings(objectArray)
                    End If

                    objectArray = {headerId, itemId, itemNumber, priceGroup}
                    If designName = "Door" Then
                        objectArray = {headerId, itemId, itemNumber, doorPriceGroup}
                    End If
                    CalculateCharge(objectArray)
                End If

                If Not String.IsNullOrEmpty(priceProductGroupIdC) Then
                    itemNumber = 3

                    objectArray = {priceProductGroupIdC, priceGroup, dropC, widthC, "Sell"}

                    Dim gridType As String = GetGridMethod(objectArray)
                    Dim gridSell As Decimal = GetGridPrice(objectArray)

                    objectArray = {priceProductGroupIdC, priceGroup, dropC, widthC, "Buy"}
                    Dim gridBuy As Decimal = GetGridPrice(objectArray)

                    Dim costSell As Decimal = gridSell
                    Dim costBuy As Decimal = gridBuy

                    thisSell = costSell
                    thisBuy = costBuy

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
                    If discountData.Rows.Count > 0 Then
                        For i As Integer = 0 To discountData.Rows.Count - 1
                            Dim discountType As String = discountData.Rows(i)("Type").ToString()
                            Dim dataId As String = discountData.Rows(i)("DataId").ToString()
                            Dim discountValue As Decimal = discountData.Rows(i)("Discount")

                            If discountType = "Designs" Then
                                If dataId = designId Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If
                                    costSell = thisSell
                                End If
                            End If

                            If discountType = "PriceProductGroups" Then
                                If dataId = priceProductGroupId Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If

                                    costSell = thisSell
                                End If
                            End If
                        Next
                    End If

                    Dim promoData As DataTable = GetDataTable("SELECT CustomerPromos.* FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    If promoData.Rows.Count > 0 Then
                        For i As Integer = 0 To promoData.Rows.Count - 1
                            Dim promoId As String = promoData.Rows(i).Item("PromoId").ToString()

                            Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")
                            If promoDetailData.Rows.Count > 0 Then
                                For iDetail As Integer = 0 To promoDetailData.Rows.Count - 1
                                    Dim promoType As String = promoDetailData.Rows(iDetail)("Type").ToString()
                                    Dim dataId As String = promoDetailData.Rows(iDetail)("DataId").ToString()
                                    Dim promoValue As Decimal = promoDetailData.Rows(iDetail)("Discount")

                                    If promoType = "Designs" Then
                                        If dataId = designId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Blinds" Then
                                        If dataId = blindId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Products" Then
                                        If dataId = productId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Fabrics" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricIdC Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "FabricColours" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricColourIdC Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If

                    If gridType = "Square Metre" Then
                        If companyId = "2" OrElse companyId = "4" OrElse companyId = "5" Then
                            thisSell = thisSell * squareMetreC
                        End If
                        If companyId = "3" Then
                            If squareMetreC < 1 Then thisSell = thisSell * 1
                            If squareMetreC >= 1 Then thisSell = thisSell * squareMetreC
                        End If
                        thisBuy = thisBuy * squareMetreD
                    End If
                    If gridType = "Linear Metre" Then
                        If companyId = "2" OrElse companyId = "4" OrElse companyId = "5" Then
                            thisSell = thisSell * linearMetreC
                        End If
                        If companyId = "3" Then
                            If linearMetreC < 1 Then thisSell = thisSell * 1
                            If linearMetreC < 1 Then thisSell = thisSell * thisSell = thisSell * linearMetreC
                        End If
                        thisBuy = thisBuy * linearMetreC
                    End If

                    Dim costingDescription As String = String.Format("#3 {0}", priceProductGroupNameC)

                    objectArray = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    OrderCostings(objectArray)

                    objectArray = {headerId, itemId, itemNumber, priceGroup}
                    CalculateCharge(objectArray)
                End If

                If Not String.IsNullOrEmpty(priceProductGroupIdD) Then
                    itemNumber = 4

                    objectArray = {priceProductGroupIdD, priceGroup, dropD, widthD, "Sell"}

                    Dim gridType As String = GetGridMethod(objectArray)
                    Dim gridSell As Decimal = GetGridPrice(objectArray)

                    objectArray = {priceProductGroupIdD, priceGroup, dropD, widthD, "Buy"}
                    Dim gridBuy As Decimal = GetGridPrice(objectArray)

                    Dim costSell As Decimal = gridSell
                    Dim costBuy As Decimal = gridBuy

                    thisSell = costSell
                    thisBuy = costBuy

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
                    If discountData.Rows.Count > 0 Then
                        For i As Integer = 0 To discountData.Rows.Count - 1
                            Dim discountType As String = discountData.Rows(i)("Type").ToString()
                            Dim dataId As String = discountData.Rows(i)("DataId").ToString()
                            Dim discountValue As Decimal = discountData.Rows(i)("Discount")

                            If discountType = "Designs" Then
                                If dataId = designId Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If
                                    costSell = thisSell
                                End If
                            End If

                            If discountType = "PriceProductGroups" Then
                                If dataId = priceProductGroupIdD Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If

                                    costSell = thisSell
                                End If
                            End If
                        Next
                    End If

                    Dim promoData As DataTable = GetDataTable("SELECT CustomerPromos.* FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    If promoData.Rows.Count > 0 Then
                        For i As Integer = 0 To promoData.Rows.Count - 1
                            Dim promoId As String = promoData.Rows(i)("PromoId").ToString()

                            Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")
                            If promoDetailData.Rows.Count > 0 Then
                                For iDetail As Integer = 0 To promoDetailData.Rows.Count - 1
                                    Dim promoType As String = promoDetailData.Rows(iDetail)("Type").ToString()
                                    Dim dataId As String = promoDetailData.Rows(iDetail)("DataId").ToString()
                                    Dim promoValue As Decimal = promoDetailData.Rows(iDetail)("Discount")

                                    If promoType = "Designs" Then
                                        If dataId = designId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Blinds" Then
                                        If dataId = blindId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Products" Then
                                        If dataId = productId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Fabrics" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricIdD Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "FabricColours" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricColourIdD Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If

                    If gridType = "Square Metre" Then
                        If companyId = "2" OrElse companyId = "4" OrElse companyId = "5" Then
                            thisSell = thisSell * squareMetreD
                        End If
                        If companyId = "3" Then
                            If squareMetreD < 1 Then thisSell = thisSell * 1
                            If squareMetreD >= 1 Then thisSell = thisSell * squareMetreD
                        End If
                        thisBuy = thisBuy * squareMetreD
                    End If
                    If gridType = "Linear Metre" Then
                        If companyId = "2" OrElse companyId = "4" OrElse companyId = "5" Then
                            thisSell = thisSell * linearMetreD
                        End If
                        If companyId = "3" Then
                            If linearMetreD < 1 Then thisSell = thisSell * 1
                            If linearMetreD >= 1 Then thisSell = thisSell * linearMetreD
                        End If
                        thisBuy = thisBuy * linearMetreD
                    End If

                    Dim costingDescription As String = String.Format("#4 {0}", priceProductGroupNameD)

                    objectArray = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    OrderCostings(objectArray)

                    objectArray = {headerId, itemId, itemNumber, priceGroup}
                    CalculateCharge(objectArray)
                End If

                If Not String.IsNullOrEmpty(priceProductGroupIdE) Then
                    itemNumber = 5

                    objectArray = {priceProductGroupIdE, priceGroup, dropE, widthE, "Sell"}

                    Dim gridType As String = GetGridMethod(objectArray)
                    Dim gridSell As Decimal = GetGridPrice(objectArray)

                    objectArray = {priceProductGroupIdE, priceGroup, dropE, widthE, "Buy"}
                    Dim gridBuy As Decimal = GetGridPrice(objectArray)

                    Dim costSell As Decimal = gridSell
                    Dim costBuy As Decimal = gridBuy

                    thisSell = costSell
                    thisBuy = costBuy

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
                    If discountData.Rows.Count > 0 Then
                        For i As Integer = 0 To discountData.Rows.Count - 1
                            Dim discountType As String = discountData.Rows(i)("Type").ToString()
                            Dim dataId As String = discountData.Rows(i)("DataId").ToString()
                            Dim discountValue As Decimal = discountData.Rows(i)("Discount")

                            If discountType = "Designs" Then
                                If dataId = designId Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If
                                    costSell = thisSell
                                End If
                            End If

                            If discountType = "PriceProductGroups" Then
                                If dataId = priceProductGroupId Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If

                                    costSell = thisSell
                                End If
                            End If
                        Next
                    End If

                    Dim promoData As DataTable = GetDataTable("SELECT CustomerPromos.* FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    If promoData.Rows.Count > 0 Then
                        For i As Integer = 0 To promoData.Rows.Count - 1
                            Dim promoId As String = promoData.Rows(i)("PromoId").ToString()

                            Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")
                            If promoDetailData.Rows.Count > 0 Then
                                For iDetail As Integer = 0 To promoDetailData.Rows.Count - 1
                                    Dim promoType As String = promoDetailData.Rows(iDetail)("Type").ToString()
                                    Dim dataId As String = promoDetailData.Rows(iDetail)("DataId").ToString()
                                    Dim promoValue As Decimal = promoDetailData.Rows(iDetail)("Discount")

                                    If promoType = "Designs" Then
                                        If dataId = designId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Blinds" Then
                                        If dataId = blindId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Products" Then
                                        If dataId = productId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Fabrics" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricIdE Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "FabricColours" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricColourIdE Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If

                    If gridType = "Square Metre" Then
                        thisSell = thisSell * squareMetreE
                        thisBuy = thisBuy * squareMetreE
                    End If
                    If gridType = "Linear Metre" Then
                        thisSell = thisSell * linearMetreE
                        thisBuy = thisBuy * linearMetreE
                    End If

                    Dim costingDescription As String = String.Format("#5 {0}", priceProductGroupNameE)

                    objectArray = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    OrderCostings(objectArray)

                    objectArray = {headerId, itemId, itemNumber, priceGroup}
                    CalculateCharge(objectArray)
                End If

                If Not String.IsNullOrEmpty(priceProductGroupIdF) Then
                    itemNumber = 6

                    objectArray = {priceProductGroupIdF, priceGroup, dropF, widthF, "Sell"}

                    Dim gridType As String = GetGridMethod(objectArray)
                    Dim gridSell As Decimal = GetGridPrice(objectArray)

                    objectArray = {priceProductGroupIdF, priceGroup, dropF, widthF, "Buy"}
                    Dim gridBuy As Decimal = GetGridPrice(objectArray)

                    Dim costSell As Decimal = gridSell
                    Dim costBuy As Decimal = gridBuy

                    thisSell = costSell
                    thisBuy = costBuy

                    Dim discountData As DataTable = GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
                    If discountData.Rows.Count > 0 Then
                        For i As Integer = 0 To discountData.Rows.Count - 1
                            Dim discountType As String = discountData.Rows(i)("Type").ToString()
                            Dim dataId As String = discountData.Rows(i)("DataId").ToString()
                            Dim discountValue As Decimal = discountData.Rows(i)("Discount")

                            If discountType = "Designs" Then
                                If dataId = designId Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If
                                    costSell = thisSell
                                End If
                            End If

                            If discountType = "PriceProductGroups" Then
                                If dataId = priceProductGroupId Then
                                    Dim baseValue As Decimal = costSell * discountValue / 100
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        baseValue = (costSell - 7) * discountValue / 100
                                    End If

                                    thisSell = costSell - baseValue
                                    If designName = "Roller Blind" AndAlso (tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Sunboss") OrElse tubeName = "Standard") Then
                                        thisSell = (costSell - 7) - baseValue + 7
                                    End If

                                    costSell = thisSell
                                End If
                            End If
                        Next
                    End If

                    Dim promoData As DataTable = GetDataTable("SELECT CustomerPromos.* FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "' AND Promos.Active=1 AND CONVERT(DATE, Promos.StartDate)<=CONVERT(DATE, GETDATE()) AND CONVERT(DATE, Promos.EndDate)>=CONVERT(DATE, GETDATE())")
                    If promoData.Rows.Count > 0 Then
                        For i As Integer = 0 To promoData.Rows.Count - 1
                            Dim promoId As String = promoData.Rows(i)("PromoId").ToString()

                            Dim promoDetailData As DataTable = GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")
                            If promoDetailData.Rows.Count > 0 Then
                                For iDetail As Integer = 0 To promoDetailData.Rows.Count - 1
                                    Dim promoType As String = promoDetailData.Rows(iDetail).Item("Type").ToString()
                                    Dim dataId As String = promoDetailData.Rows(iDetail).Item("DataId").ToString()
                                    Dim promoValue As Decimal = promoDetailData.Rows(iDetail).Item("Discount")

                                    If promoType = "Designs" Then
                                        If dataId = designId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Blinds" Then
                                        If dataId = blindId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Products" Then
                                        If dataId = productId Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "Fabrics" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricIdF Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If

                                    If promoType = "FabricColours" AndAlso designName = "Roller Blind" Then
                                        If dataId = fabricColourIdF Then
                                            Dim baseValue As Decimal = costSell * promoValue / 100

                                            thisSell = costSell - baseValue
                                            costSell = thisSell
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If

                    If gridType = "Square Metre" Then
                        thisSell = thisSell * squareMetreE
                        thisBuy = thisBuy * squareMetreE
                    End If
                    If gridType = "Linear Metre" Then
                        thisSell = thisSell * linearMetreE
                        thisBuy = thisBuy * linearMetreE
                    End If

                    Dim costingDescription As String = String.Format("#6 {0}", priceProductGroupNameF)

                    objectArray = {headerId, itemId, itemNumber, "Base", costingDescription, thisBuy, thisSell}
                    OrderCostings(objectArray)

                    objectArray = {headerId, itemId, itemNumber, priceGroup}
                    CalculateCharge(objectArray)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub CalculateCharge(data As Object())
        Try
            Dim headerId As String = Convert.ToString(data(0))
            Dim itemId As String = Convert.ToString(data(1))
            Dim blindNumber As String = Convert.ToString(data(2))
            Dim priceGroup As String = Convert.ToString(data(3))

            Dim thisData As DataRow = GetDataRow("SELECT * FROM OrderDetails WHERE Id='" & itemId & "' AND Active=1 ORDER BY Id ASC")
            If thisData IsNot Nothing Then
                Dim productId As String = thisData("ProductId").ToString()
                Dim designId As String = GetDesignId(productId)
                Dim blindId As String = GetBlindId(productId)

                Dim surchargeData As DataTable = GetDataTable("SELECT * FROM PriceSurcharges WHERE DesignId='" & designId & "' AND BlindId='" + blindId + "' AND BlindNumber='" & blindNumber & "' AND PriceGroupId='" & priceGroup & "' AND Active=1 ORDER BY Id ASC")
                If surchargeData.Rows.Count > 0 Then
                    For i As Integer = 0 To surchargeData.Rows.Count - 1
                        Dim id As String = surchargeData.Rows(i)("Id").ToString()
                        Dim name As String = surchargeData.Rows(i)("Name").ToString()
                        Dim fieldName As String = surchargeData.Rows(i)("FieldName").ToString()
                        Dim formula As String = surchargeData.Rows(i).Item("Formula").ToString()
                        Dim buyCharge As String = surchargeData.Rows(i)("BuyCharge").ToString()
                        Dim sellCharge As String = surchargeData.Rows(i)("SellCharge").ToString()
                        Dim description As String = surchargeData.Rows(i)("Description").ToString()

                        Dim thisBuy As Decimal = 0.00
                        Dim thisSell As Decimal = 0.00

                        Dim cekFormula As String = GetItemData("SELECT " + fieldName + " FROM viewSurcharge WHERE Id='" + itemId + "' AND " + formula)
                        If Not cekFormula = "" Then
                            Dim queryBuy As String = "SELECT " + buyCharge + " FROM viewSurcharge WHERE Id='" + itemId + "'"
                            Dim querySell As String = "SELECT " + sellCharge + " FROM viewSurcharge WHERE Id='" + itemId + "'"

                            thisBuy = GetItemData_Decimal(queryBuy)
                            thisSell = GetItemData_Decimal(querySell)

                            Dim dataCosting As Object() = {headerId, itemId, blindNumber, "Surcharge", name, thisBuy, thisSell}
                            OrderCostings(dataCosting)
                        End If
                    Next
                End If
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
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderCostings VALUES(NEWID(), @HeaderId, @ItemId, @Number, @Type, @Description, @BuyPrice, @SellPrice)", thisConn)
                        myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                        myCmd.Parameters.AddWithValue("@ItemId", itemId)
                        myCmd.Parameters.AddWithValue("@Number", number)
                        myCmd.Parameters.AddWithValue("@Type", type)
                        myCmd.Parameters.AddWithValue("@Description", desc)
                        myCmd.Parameters.AddWithValue("@BuyPrice", buyPrice)
                        myCmd.Parameters.AddWithValue("@SellPrice", sellPrice)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
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
                    Using myCmd As New SqlCommand("sp_InsertLogs", thisConn)
                        myCmd.CommandType = CommandType.StoredProcedure

                        myCmd.Parameters.AddWithValue("@Type", Convert.ToString(data(0)))
                        myCmd.Parameters.AddWithValue("@DataId", Convert.ToString(data(1)))
                        myCmd.Parameters.AddWithValue("@ActionBy", Convert.ToString(data(2)))
                        myCmd.Parameters.AddWithValue("@Description", Convert.ToString(data(3)))

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
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
                    Dim query As String = "SELECT COUNT(*) FROM OrderReworks WHERE HeaderIdNew=@HeaderId AND Active=1"
                    Using myCmd As New SqlCommand(query, thisConn)
                        myCmd.Parameters.AddWithValue("@HeaderId", orderId)
                        thisConn.Open()

                        Dim count As Integer = Convert.ToInt32(myCmd.ExecuteScalar())
                        result = (count > 0)
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Function CheckRework(headerId As String) As Integer
        Dim result As Integer = 0
        Try
            If Not String.IsNullOrEmpty(headerId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("SELECT COUNT(*) FROM OrderReworks WHERE HeaderId='" & headerId & "' AND (Status='Pending Approval' OR Status='Approved') AND Active=1", thisConn)
                        myCmd.Parameters.AddWithValue("@OrderId", headerId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function


    ' SHUTTER
    Public Function CountMultiLayout(input As String, substrings As String()) As Integer
        Dim count As Integer = 0
        For Each substring In substrings
            count += input.Split(substring).Length - 1
        Next
        Return count
    End Function

    Public Function CheckStringLayoutD(layout As String) As Boolean
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
