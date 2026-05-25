Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_ProductAccess
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomerProductAccess")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Session("SearchCustomerAddress") = txtSearch.Text
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerProductAccess WHERE Id='" & dataId & "'")
                    If myData Is Nothing Then Exit Sub

                    Dim companyId As String = settingClass.GetItemData("SELECT CompanyId FROM Customers WHERE Id='" & dataId & "'")

                    BindProduct(companyId)

                    Dim tagsArray() As String = myData("DesignId").ToString().Split(",")
                    Dim tagsList As List(Of String) = tagsArray.ToList()
                    For Each i In tagsArray
                        If Not (i.Equals(String.Empty)) Then
                            lbProduct.Items.FindByValue(i).Selected = True
                        End If
                    Next

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            Dim designId As String = String.Empty
            Dim tags As String = String.Empty
            For Each item As ListItem In lbProduct.Items
                If item.Selected Then
                    tags += item.Value.ToString() & ","
                End If
            Next
            If Not tags = "" Then
                designId = tags.Remove(tags.Length - 1).ToString()
            End If

            If msgErrorProcess.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerProductAccess SET DesignId=@DesignId WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@DesignId", designId)

                        myCmd.ExecuteNonQuery()
                    End Using

                    thisConn.Close()
                End Using

                dataLog = {"CustomerProductAccess", lblId.Text, Session("LoginId").ToString(), "Customer Product Access Updated"}
                settingClass.Logs(dataLog)

                Session("SearchCustomerProductAccess") = txtSearch.Text
                Response.Redirect("~/setting/customer/productaccess", False)
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnReset_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdReset.Text
            Dim companyId As String = settingClass.GetItemData("SELECT CompanyId FROM Customers WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                Dim desingId As String = settingClass.GetProductAccess(companyId)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerProductAccess SET DesignId=@DesignId WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@DesignId", desingId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"CustomerProductAccess", thisId, Session("LoginId").ToString(), "Reset Customer Product Access"}
            settingClass.Logs(dataLog)

            Session("SearchCustomerProductAccess") = txtSearch.Text
            Response.Redirect("~/setting/customer/productaccess", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerProductAccess") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText.Trim())),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing, CType(DBNull.Value, Object), Session("CompanyId"))),
                New SqlParameter("@LoginId", Session("LoginId"))
            }

            gvList.DataSource = settingClass.GetDataTableSP("sp_CustomerProductAccess", params)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindProduct(companyId As String)
        lbProduct.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                lbProduct.DataSource = settingClass.GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray WHERE companyArray.VALUE='" & companyId & "' ORDER BY Name ASC")
                lbProduct.DataTextField = "Name"
                lbProduct.DataValueField = "Id"
                lbProduct.DataBind()
                If lbProduct.Items.Count > 1 Then
                    lbProduct.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            lbProduct.Items.Clear()
        End Try
    End Sub

    Protected Function BindDetailProduct(customerId As String) As String
        Dim result As String = String.Empty
        Try
            Dim hasil As String = String.Empty
            Dim myData As DataTable = settingClass.GetDataTable("SELECT Designs.Name AS DesignName FROM CustomerProductAccess CROSS APPLY STRING_SPLIT(CustomerProductAccess.DesignId, ',') AS designArray LEFT JOIN Designs ON designArray.VALUE=Designs.Id WHERE CustomerProductAccess.Id='" & customerId & "' ORDER BY Designs.Name ASC ")
            If Not myData.Rows.Count = 0 Then
                For i As Integer = 0 To myData.Rows.Count - 1
                    Dim designName As String = myData.Rows(i)("DesignName").ToString()
                    hasil += designName & ", "
                Next
            End If

            result = hasil.Remove(hasil.Length - 2).ToString()
        Catch ex As Exception
            result = "Error"
        End Try
        Return result
    End Function

    Private Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
