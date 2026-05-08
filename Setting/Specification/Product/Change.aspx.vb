Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Change
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
            MessageError(False, String.Empty)

            BindDesignType()
            BindBlindType(ddlDesignType.SelectedValue)
            BindTubeType()
            BindControlType()
            BindColourType()
        End If
    End Sub

    Protected Sub ddlDesignType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindBlindType(ddlDesignType.SelectedValue)
    End Sub

    Protected Sub ddlBlindType_SelectedIndexChanged(sender As Object, e As EventArgs)

    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDesignType.SelectedValue = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim blindString As String = String.Empty
                If Not ddlBlindType.SelectedValue = "" Then blindString = "AND BlindId='" & ddlBlindType.SelectedValue & "'"
                Dim tubeString As String = String.Empty
                If Not ddlTubeType.SelectedValue = "" Then tubeString = "AND TubeType='" & ddlTubeType.SelectedValue & "'"
                Dim controlString As String = String.Empty
                If Not ddlControlType.SelectedValue = "" Then controlString = "AND ControlType='" & ddlControlType.SelectedValue & "'"
                Dim colourString As String = String.Empty
                If Not ddlColourType.SelectedValue = "" Then colourString = "AND ColourType='" & ddlColourType.SelectedValue & "'"

                Dim changeString As String = String.Format("UPDATE Products SET Status=@Status WHERE Status<>'' AND DesignId=@DesignId {0} {1} {2} {3}", blindString, tubeString, controlString, colourString)

                Dim thisData As DataTable = settingClass.GetDataTable("")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand(changeString, thisConn)
                        myCmd.Parameters.AddWithValue("@DesignId", ddlDesignType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@BlindId", ddlBlindType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@TubeType", ddlTubeType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@ControlType", ddlControlType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@ColourType", ddlColourType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/product", False)
    End Sub

    Protected Sub BindDesignType()
        ddlDesignType.Items.Clear()
        Try
            ddlDesignType.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            ddlDesignType.DataTextField = "Name"
            ddlDesignType.DataValueField = "Id"
            ddlDesignType.DataBind()

            If ddlDesignType.Items.Count > 1 Then
                ddlDesignType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindBlindType(designId As String)
        ddlBlindType.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(designId) Then
                ddlBlindType.DataSource = settingClass.GetDataTable("SELECT * FROM Blinds WHERE DesignId='" & designId & "' ORDER BY Name ASC")
                ddlBlindType.DataTextField = "Name"
                ddlBlindType.DataValueField = "Id"
                ddlBlindType.DataBind()

                If ddlBlindType.Items.Count > 1 Then
                    ddlBlindType.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindTubeType()
        ddlTubeType.Items.Clear()
        Try
            ddlTubeType.DataSource = settingClass.GetDataTable("SELECT * FROM ProductTubes ORDER BY Name ASC")
            ddlTubeType.DataTextField = "Name"
            ddlTubeType.DataValueField = "Id"
            ddlTubeType.DataBind()

            If ddlTubeType.Items.Count > 1 Then
                ddlTubeType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindControlType()
        ddlControlType.Items.Clear()
        Try
            ddlControlType.DataSource = settingClass.GetDataTable("SELECT * FROM ProductControls ORDER BY Name ASC")
            ddlControlType.DataTextField = "Name"
            ddlControlType.DataValueField = "Id"
            ddlControlType.DataBind()

            If ddlControlType.Items.Count > 1 Then
                ddlControlType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindColourType()
        ddlColourType.Items.Clear()
        Try
            ddlColourType.DataSource = settingClass.GetDataTable("SELECT * FROM ProductColours ORDER BY Name ASC")
            ddlColourType.DataTextField = "Name"
            ddlColourType.DataValueField = "Id"
            ddlColourType.DataBind()

            If ddlColourType.Items.Count > 1 Then
                ddlColourType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
