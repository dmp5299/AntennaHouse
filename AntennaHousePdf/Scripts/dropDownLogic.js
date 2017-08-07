window.onload = function (e) {
project = document.getElementById("baseProject");
subProject = document.getElementById("sbs");
if (project.options[project.selectedIndex].value == "CMM") {
    document.getElementById("CMMproject").style.display = "block";
    document.getElementById("Rolls Royceproject").style.display = "none";
    document.getElementById("SBProject").style.display = "none";
    document.getElementById("Footer").style.display = "none";

}
else if (project.options[project.selectedIndex].value == "Rolls Royce") {
    document.getElementById("Rolls Royceproject").style.display = "block";
    document.getElementById("CMMproject").style.display = "none";
    document.getElementById("SBProject").style.display = "none";
    document.getElementById("Footer").style.display = "none";
}
else if (project.options[project.selectedIndex].value == "SB") {
    if (subProject.options[subProject.selectedIndex].value == "Utas") {
        document.getElementById("Footer").style.display = "none";
    }
    else {
        document.getElementById("Footer").style.display = "block";
    }
    document.getElementById("SBProject").style.display = "block";
    document.getElementById("CMMproject").style.display = "none";
    document.getElementById("Rolls Royceproject").style.display = "none";
}
else if (project.options[project.selectedIndex].value == "PWC") {
    document.getElementById("SBProject").style.display = "none";
    document.getElementById("CMMproject").style.display = "none";
    document.getElementById("Rolls Royceproject").style.display = "none";
    document.getElementById("Footer").style.display = "none";
}
else if (project.options[project.selectedIndex].value == "53K") {
    document.getElementById("SBProject").style.display = "none";
    document.getElementById("CMMproject").style.display = "none";
    document.getElementById("Rolls Royceproject").style.display = "none";
    document.getElementById("Footer").style.display = "none";
}
}

function checkFiles() {
var filePath = document.getElementById('sbXml').value;
if (filePath.length < 1) {
    alert("Select one or more xml files");
    return false;
}
}

function change_project(project) {
if (project.options[project.selectedIndex].value == "CMM") {
    document.getElementById(project.options[project.selectedIndex].value + "project").style.display = 'block';
    document.getElementById("Rolls Royceproject").style.display = 'none';
    document.getElementById("SBProject").style.display = 'none';
    document.getElementById("Footer").style.display = 'none';
}
else if (project.options[project.selectedIndex].value == "PWC") {
    document.getElementById("Rolls Royceproject").style.display = 'none';
    document.getElementById("CMMproject").style.display = 'none';
    document.getElementById("SBProject").style.display = 'none';
    document.getElementById("Footer").style.display = 'none';
}
else if (project.options[project.selectedIndex].value == "53K") {
    document.getElementById("Rolls Royceproject").style.display = 'none';
    document.getElementById("CMMproject").style.display = 'none';
    document.getElementById("SBProject").style.display = 'none';
    document.getElementById("Footer").style.display = 'none';
}
else if (project.options[project.selectedIndex].value == "Rolls Royce") {
    document.getElementById(project.options[project.selectedIndex].value + "project").style.display = 'block';
    document.getElementById("CMMproject").style.display = 'none';
    document.getElementById("SBProject").style.display = 'none';
    document.getElementById("Footer").style.display = 'none';
}
else if (project.options[project.selectedIndex].value == "SB") {
    var e = document.getElementById("sbs");
    document.getElementById(project.options[project.selectedIndex].value + "Project").style.display = 'block';
    document.getElementById("Rolls Royceproject").style.display = 'none';
    document.getElementById("CMMproject").style.display = 'none';
    if (e.options[e.selectedIndex].value == "Airbus") {
        document.getElementById("Footer").style.display = 'block';
    }
}
}

function change_sb(project) {
if (project.options[project.selectedIndex].value == "Airbus") {
    document.getElementById("Footer").style.display = 'block';
}
else {
    document.getElementById("Footer").style.display = 'none';
}

}