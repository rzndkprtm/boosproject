Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Customer_Promo
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomerPromo")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            BindDataCustomer()
            BindDataPromo()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("SearchCustomerPromo") = txtSearch.Text
            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError_Detail(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showDetail(); };"
                Try
                    Dim promoId As String = settingClass.GetItemData("SELECT PromoId FROM CustomerPromos WHERE Id='" & dataId & "'")
                    gvListDetail.DataSource = settingClass.GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")
                    gvListDetail.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetail", thisScript, True)
                Catch ex As Exception
                    MessageError_Detail(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetail", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError_Process(True, "CUSTOMER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlPromo.SelectedValue = "" Then
                MessageError_Process(True, "PROMO IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            Dim checkData As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) FROM CustomerPromos WHERE CustomerId='" & lblId.Text & "' AND PromoId='" & ddlPromo.SelectedValue & "'")
            If checkData > 0 Then
                MessageError_Process(True, "THIS PROMO IS ALREADY REGISTERED. PLEASE USE A DIFFERENT PROMO !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerPromos ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerPromos VALUES (@Id, @CustomerId, @PromoId)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                        myCmd.Parameters.AddWithValue("@PromoId", ddlPromo.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"CustomerPromos", thisId, Session("LoginId").ToString(), "Customer Promo Created"}
                settingClass.Logs(dataLog)

                Session("SearchCustomerPromo") = txtSearch.Text
                Response.Redirect("~/setting/customer/promo", False)
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Dim customerId As String = settingClass.GetItemData("SELECT CustomerId FROM CustomerPromos WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerPromos WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerPromos' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            dataLog = {"Customers", customerId, Session("LoginId"), "Customer Promo Deleted"}
            settingClass.Logs(dataLog)

            Session("SearchCustomerPromo") = txtSearch.Text
            Response.Redirect("~/setting/customer/promo", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerPromo") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not String.IsNullOrEmpty(searchText) Then
                search = "WHERE Customers.Name LIKE '%" & searchText.Trim() & "%' OR Customers.DebtorCode LIKE '%" & searchText.Trim() & "%' OR Promos.Name LIKE '%" & searchText.Trim() & "%'"
            End If
            Dim thisQuery As String = String.Format("SELECT CustomerPromos.*, Customers.Name AS CustomerName, Promos.Name AS PromoName FROM CustomerPromos LEFT JOIN Customers ON CustomerPromos.CustomerId=Customers.Id LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id {0} ORDER BY Customers.Id, CustomerPromos.Id ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID") ' ID
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindDataCustomer()
        ddlCustomer.Items.Clear()
        Try
            ddlCustomer.DataSource = settingClass.GetDataTable("SELECT * FROM Customers WHERE Active=1 ORDER BY Name ASC")
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCustomer.Items.Clear()
        End Try
    End Sub

    Protected Sub BindDataPromo()
        ddlPromo.Items.Clear()
        Try
            ddlPromo.DataSource = settingClass.GetDataTable("SELECT * FROM Promos WHERE Active=1 ORDER BY Name ASC")
            ddlPromo.DataTextField = "Name"
            ddlPromo.DataValueField = "Id"
            ddlPromo.DataBind()

            If ddlPromo.Items.Count > 1 Then
                ddlPromo.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlPromo.Items.Clear()
        End Try
    End Sub

    Protected Function PromoTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        If type = "FrameColours" Then Return dataId
        Return settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", type, dataId))
    End Function

    Protected Function PromoValue(data As Decimal) As String
        If data > 0 Then Return data.ToString("G29", enUS) & "%"
        Return "ERROR"
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

    Protected Sub MessageError_Detail(visible As Boolean, message As String)
        divErrorDetail.Visible = visible : msgErrorDetail.InnerText = message
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
