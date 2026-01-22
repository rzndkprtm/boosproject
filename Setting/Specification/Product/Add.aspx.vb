Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Add
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/product", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            BackColor()
            BindDesign()
            BindBlind(ddlDesign.SelectedValue)
            BindCompanyDetail(ddlBlind.SelectedValue)
            BindControl()
            BindTube()
            BindColour()
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        BackColor()
        BindBlind(ddlDesign.SelectedValue)
        BindCompanyDetail(ddlBlind.SelectedValue)
    End Sub

    Protected Sub ddlBlind_SelectedIndexChanged(sender As Object, e As EventArgs)
        BackColor()
        BindCompanyDetail(ddlBlind.SelectedValue)
    End Sub

    Protected Sub btnAddControl_Click(sender As Object, e As EventArgs)
        MessageError_Control(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showControl(); };"
        Try
            ClientScript.RegisterStartupScript(Me.GetType(), "showControl", thisScript, True)
        Catch ex As Exception
            MessageError_Control(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showControl", thisScript, True)
        End Try
    End Sub

    Protected Sub btnAddTube_Click(sender As Object, e As EventArgs)
        MessageError_Tube(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showTube(); };"
        Try
            ClientScript.RegisterStartupScript(Me.GetType(), "showTube", thisScript, True)
        Catch ex As Exception
            MessageError_Tube(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showTube", thisScript, True)
        End Try
    End Sub

    Protected Sub btnAddColour_Click(sender As Object, e As EventArgs)
        MessageError_Colour(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showColour(); };"
        Try
            ClientScript.RegisterStartupScript(Me.GetType(), "showColour", thisScript, True)
        Catch ex As Exception
            MessageError_Colour(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showColour", thisScript, True)
        End Try
    End Sub

    Protected Sub btnTube_Click(sender As Object, e As EventArgs)
        MessageError_Tube(False, String.Empty)
        Try
            If msgErrorProcessTube.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM ProductTubes ORDER BY Id DESC")
                Dim descText As String = txtTubeDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO ProductTubes VALUES (@Id, @Name, @Description)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Name", txtTubeName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Description", descText)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"ProductTubes", thisId, Session("LoginId").ToString(), "Product Tube Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/specification/product/add", False)
            End If
        Catch ex As Exception
            MessageError_Tube(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnControl_Click(sender As Object, e As EventArgs)
        MessageError_Control(False, String.Empty)
        Try
            If msgErrorProcessControl.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM ProductControls ORDER BY Id DESC")
                Dim descText As String = txtControlDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO ProductControls VALUES (@Id, @Name, @Description)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Name", txtControlName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Description", descText)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"ProductControls", thisId, Session("LoginId").ToString(), "Product Control Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/specification/product/add", False)
            End If
        Catch ex As Exception
            MessageError_Control(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnColour_Click(sender As Object, e As EventArgs)
        MessageError_Colour(False, String.Empty)
        Try
            If msgErrorProcessColour.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM ProductColours ORDER BY Id DESC")
                Dim descText As String = txtColourDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO ProductColours VALUES (@Id, @Name, @Description)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Name", txtColourName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Description", descText)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"ProductColours", thisId, Session("LoginId").ToString(), "Product Colour Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/specification/product/add", False)
            End If
        Catch ex As Exception
            MessageError_Colour(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If ddlDesign.SelectedValue = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If

            If ddlBlind.SelectedValue = "" Then
                MessageError(True, "BLIND TYPE IS REQUIRED !")
                Exit Sub
            End If

            Dim company As String = String.Empty
            For Each item As ListItem In lbCompanyDetail.Items
                If item.Selected Then
                    company += item.Value & ","
                End If
            Next
            If company = "" Then
                MessageError(True, "SUB COMPANY IS REQUIRED !")
                Exit Sub
            End If

            If txtName.Text = "" Then
                MessageError(True, "NAME IS REQUIRED !")
                Exit Sub
            End If

            If ddlControl.SelectedValue = "" Then
                MessageError(True, "CONTROL TYPE IS REQUIRED !")
                Exit Sub
            End If

            If ddlTube.SelectedValue = "" Then
                MessageError(True, "TUBE TYPE IS REQUIRED !")
                Exit Sub
            End If

            If ddlColour.SelectedValue = "" Then
                MessageError(True, "COLOUR TYPE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim companyDetailId As String = company.Remove(company.Length - 1).ToString()
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                If String.IsNullOrEmpty(txtInvoiceName.Text) Then txtInvoiceName.Text = txtName.Text

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Products ORDER BY Id DESC")
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Products VALUES (@Id, @DesignId, @BlindId, @CompanyDetailId, @Name, @InvoiceName, @TubeType, @ControlType, @ColourType, @Description, @Active)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@DesignId", ddlDesign.SelectedValue)
                        myCmd.Parameters.AddWithValue("@BlindId", ddlBlind.SelectedValue)
                        myCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetailId)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text)
                        myCmd.Parameters.AddWithValue("@InvoiceName", txtInvoiceName.Text)
                        myCmd.Parameters.AddWithValue("@TubeType", ddlTube.SelectedValue)
                        myCmd.Parameters.AddWithValue("@ControlType", ddlControl.SelectedValue)
                        myCmd.Parameters.AddWithValue("@ColourType", ddlColour.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Description", descText)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Products", thisId, Session("LoginId").ToString(), "Product Created"}
                settingClass.Logs(dataLog)

                Dim url As String = String.Format("~/setting/specification/product/detail?productid={0}", thisId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/product", False)
    End Sub

    Protected Sub BindDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            ddlDesign.DataTextField = "Name"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 1 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlDesign.Items.Clear()
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindBlind(designId As String)
        ddlBlind.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(designId) Then
                ddlBlind.DataSource = settingClass.GetDataTable("SELECT * FROM Blinds WHERE DesignId='" & designId & "' ORDER BY Name ASC")
                ddlBlind.DataTextField = "Name"
                ddlBlind.DataValueField = "Id"
                ddlBlind.DataBind()

                If ddlBlind.Items.Count > 1 Then
                    ddlBlind.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlBlind.Items.Clear()
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindCompanyDetail(blindId As String)
        lbCompanyDetail.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(blindId) Then
                lbCompanyDetail.DataSource = settingClass.GetDataTable("SELECT CompanyDetails.* FROM Blinds CROSS APPLY STRING_SPLIT(Blinds.CompanyDetailId, ',') AS thisArray JOIN CompanyDetails ON CompanyDetails.Id=CAST(thisArray.value AS INT) WHERE Blinds.Id='" & blindId & "' ORDER BY CompanyDetails.Name ASC;")
                lbCompanyDetail.DataTextField = "Name"
                lbCompanyDetail.DataValueField = "Id"
                lbCompanyDetail.DataBind()

                If lbCompanyDetail.Items.Count > 0 Then
                    lbCompanyDetail.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            lbCompanyDetail.Items.Clear()
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindTube()
        ddlTube.Items.Clear()
        Try
            ddlTube.DataSource = settingClass.GetDataTable("SELECT * FROM ProductTubes ORDER BY Name ASC")
            ddlTube.DataTextField = "Name"
            ddlTube.DataValueField = "Id"
            ddlTube.DataBind()

            If ddlTube.Items.Count > 1 Then
                ddlTube.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlTube.Items.Clear()
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindControl()
        ddlControl.Items.Clear()
        Try
            ddlControl.DataSource = settingClass.GetDataTable("SELECT * FROM ProductControls ORDER BY Name ASC")
            ddlControl.DataTextField = "Name"
            ddlControl.DataValueField = "Id"
            ddlControl.DataBind()

            If ddlControl.Items.Count > 1 Then
                ddlControl.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlControl.Items.Clear()
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindColour()
        ddlColour.Items.Clear()
        Try
            ddlColour.DataSource = settingClass.GetDataTable("SELECT * FROM ProductColours ORDER BY Name ASC")
            ddlColour.DataTextField = "Name"
            ddlColour.DataValueField = "Id"
            ddlColour.DataBind()

            If ddlColour.Items.Count > 1 Then
                ddlColour.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlColour.Items.Clear()
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BackColor()
        MessageError(False, String.Empty)
        MessageError_Control(False, String.Empty)
        MessageError_Tube(False, String.Empty)
        MessageError_Colour(False, String.Empty)
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Control(visible As Boolean, message As String)
        divErrorProcessControl.Visible = visible : msgErrorProcessControl.InnerText = message
    End Sub

    Protected Sub MessageError_Tube(visible As Boolean, message As String)
        divErrorProcessTube.Visible = visible : msgErrorProcessTube.InnerText = message
    End Sub

    Protected Sub MessageError_Colour(visible As Boolean, message As String)
        divErrorProcessColour.Visible = visible : msgErrorProcessColour.InnerText = message
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
