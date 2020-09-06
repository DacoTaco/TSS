<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditTask.aspx.cs" Inherits="TSS_ASPWebForms.EditTask" %>
<%@ Register assembly="TechnicalServiceSystem.UI" namespace="TechnicalServiceSystem.UI.HTML" tagprefix="web" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TSS : Technical Service System</title>
    <link href="Content/bootstrap.css" rel="stylesheet" type="text/css"/>
    <link href="CSS/Style.css" rel="stylesheet" type="text/css"/>
    <link href="CSS/EditTask.css" rel="stylesheet" type="text/css"/>
    <script src="Scripts/jquery-3.1.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>
    <script src="Scripts/TSS.js"></script>
    <script src="Scripts/TSS-EditTask.js"></script>
    <link href="CSS/lightbox.css" rel="stylesheet" type="text/css"/>
    <script src="Scripts/lightbox.js"></script>

    <meta name="viewport" content="initial-scale=1.0, user-scalable=no"/>
    <meta name="google" content="notranslate"/>
</head>
<body>
<div class="centerPage container-fluid" id="MainContainer" style="background-color: lightblue; border: solid; border-width: 2px;">
    <form id="EditTaskForm" runat="server">
        <div id="EditTask">
            <div id="loadingTaskDiv" class="blocker" hidden="hidden" runat="server" style="z-index: 999;">
                <div class="blocker-content">
                    <asp:Image ID="loadingImage" runat="server" ImageAlign="AbsMiddle" ImageUrl="/system/loading.gif" style="height: auto; width: 3em; z-index: 9999;"/>
                </div>
            </div>
            <!-- Show the read only msg if needed -->
            <script type="text/javascript" id="ReadOnly">
                function myFunction() {
                    if ('True' == '<%# ReadOnly %>')
                        alert('<%# ReadOnlyMsg %>');
                }
            </script>
            
            <table style="margin-bottom: 10px; margin-left: auto; margin-right: auto; table-layout: fixed; width: 100%;" id="TaskTable">
                <tr>
                    <!-- amount of columns -->
                    <th style="width: auto;"></th>
                    <th style="width: 30%;"></th>
                    <th style="width: 17%;"></th>
                    <th style="width: 27%;"></th>
                </tr>
                <!-- columns! -->
                <tr>
                    <!-- Reporter name & Technician row -->
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                        <asp:Label runat="server" Text="Reporter : " ID="lblReporter" meta:resourcekey="Reporter" Width="100%"></asp:Label>
                    </td>
                    <td style="height: 5px; padding-bottom: 1em; padding-right: 1em; text-align: left;">
                        <input type="text" runat="server" id="txtReporter" value="<%# Task.Reporter %>" autocomplete="off" onblur="updateTaskReporter(); return false;" style="height: 2em; max-height: inherit; min-height: 5px; width: 100%;"/>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                        <asp:Label runat="server" Text="Technician : " meta:resourcekey="Technician" Width="100%"></asp:Label>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left;">
                        <web:selectObject runat="server" id="selectTechnicians" style="width: 100%" DataValueField="ID" DataTextField="UserName" onchange="updateTaskTechnician(); return false;"/>
                    </td>
                </tr>
                <tr>
                    <!-- department & location row -->
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                        <asp:Label runat="server" Text="Department : " meta:resourcekey="Department" Width="100%"></asp:Label>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left;">
                        <web:selectObject runat="server" id="selectDepartments" style="width: 100%;" DataValueField="ID" DataTextField="Description" onchange="updateTaskDepartment(); return false;"/>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                        <asp:Label runat="server" Text="Location : " meta:resourcekey="Location" Width="100%"></asp:Label>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left;">
                        <div id="locations">
                            <web:selectObject runat="server" id="selectLocations" style="width: 100%" DataValueField="ID" DataTextField="Description" onchange="updateTaskLocation(); return false;"/>
                        </div>
                    </td>
                </tr>
                <tr>
                    <!-- Description row -->
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                        <asp:Label runat="server" Text="Description : " meta:resourcekey="Description" Width="100%"></asp:Label>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left;" colspan="3">
                        <input type="text" runat="server" id="txtDescription" value="<%# Task.Description %>" onblur="updateTaskDescription(); return false;" autocomplete="off" style="height: 2em; max-height: inherit; min-height: 5px; width: 100%;"/>
                    </td>
                </tr>
                <tr>
                    <!-- Notes row -->
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right; vertical-align: top;">
                        <asp:Label runat="server" Text="Notes : " meta:resourcekey="Notes" Width="100%"></asp:Label>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left; width: 100%;" colspan="3" rowspan="4" class="notesBox">
                        <asp:TextBox runat="server" ID="Notes" Width="100%" Text="<%# Task.strNotes %>" TextMode="MultiLine" Enabled="false" style="max-height: inherit; min-height: inherit; resize: none;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <!-- add notes row -->
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                        <asp:Label runat="server" Text="Add Notes : " meta:resourcekey="AddNotes"> </asp:Label>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left;" colspan="2">
                        <input runat="server" id="NoteBox" autocomplete="off" style="height: inherit; max-height: inherit; min-height: 5px; width: 100%"/>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left;">
                        <asp:button runat="server" id="AddNotesButton" style="width: 100%" OnClientClick="AddTaskNote();return false;" Text="Add Note" meta:resourcekey="AddNotesBtn"/> 
                    </td>
                </tr>
                <!-- Machine row -->
                <tr runat="server" id="machineRow">
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                        <asp:Label runat="server" Text="Machines : " meta:resourcekey="Machines"> </asp:Label>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left;" colspan="2">
                        <web:selectObject runat="server" id="selectMachines" style="width: 100%" DataValueField="ID" datatextfield="Description" onchange="updateTaskMachine(); return false;"/>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left;">
                        <asp:Button runat="server" Text="Documentation..." meta:resourcekey="Documentation" Width="100%" OnClientClick="alert('not yet implemented');return false;"></asp:Button>
                    </td>
                </tr>
                <tr>
                    <!-- Task State row -->
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                        <asp:Label runat="server" Text="State : " meta:resourcekey="TaskState"></asp:Label>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left;">
                        <web:selectObject runat="server" id="selectTaskState" style="width: 100%" DataValueField="ID" DataTextField="Description" onchange="updateTaskState(); return false;"/>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                        <asp:Label runat="server" Text="Urguent : " meta:resourcekey="Urguent"></asp:Label>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left; vertical-align: middle;">
                        <asp:CheckBox runat="server" ID="chkUrguent" onchange="updateTaskUrguency(); return false;" Checked="<%# Task.IsUrguent %>"/>

                    </td>
                </tr>
                <tr>
                    <!-- Photo's row -->
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right; vertical-align: top;">
                        <asp:Label runat="server" Text="Photo's : " meta:resourcekey="Photos"></asp:Label>
                    </td>
                    <td style="height: 100%; max-height: 12em; min-height: 12em; padding-bottom: 1em; padding-right: 1em; text-align: right; width: 100%;" colspan="3" rowspan="4">
                        <div style="background-color: white; border: solid; border-color: black; border-width: 1px; max-height: inherit; min-height: inherit; overflow: scroll; overflow-y: hidden; padding: 0px; white-space: nowrap;">
                            <!-- the list containing all photos -->
                            <ul runat="server" id="imagesList" style="display: inline; float: left; height: 100%; max-height: 10em; min-height: 10em; padding: 5px; text-align: left; width: 100%;">

                            </ul>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <!-- Add photos row -->
                    <td></td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;" colspan="2">
                        <asp:Button runat="server" ID="AddPhotoBtn" Text="Add Photo" meta:resourcekey="AddPhoto" style="min-width: 2em; width: 60%;"
                                    OnClientClick="$('#AddPhotoInput').click();return false;"/>

                        <input id="AddPhotoInput" runat="server" type="file" accept="image/*" capture="camera" onchange="AddTaskPhoto(this); return false;" style="visibility: hidden"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" style="font-size: 1em; margin-left: auto; margin-right: auto;">
                        <asp:Label runat="server" Text="Last Saved : " meta:resourcekey="LastAdjusted"></asp:Label>
                        <asp:Label runat="server" Text="<%# Task.LastModifiedOn %>"></asp:Label>
                    </td>
                </tr>
            </table>

        </div>
    </form>
</div>
</body>
</html>