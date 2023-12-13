using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingHover : MonoBehaviour
{
    public Text buildingDescriptionText;
    public TextAsset defaultDescription;
    public TextAsset houseDescription;
    public TextAsset forgeDescription;
    public TextAsset sawmillDescription;
    public TextAsset fieldDescription;

    private void Start()
    {
        buildingDescriptionText.text = defaultDescription.text;
    }

    public void BuildingEnter()
    {
        string type = this.name.Replace("ImageBuild", "");

        if (type == "House")
            buildingDescriptionText.text = houseDescription.text;
        if (type == "Forge")
            buildingDescriptionText.text = forgeDescription.text;
        if (type == "Sawmill")
            buildingDescriptionText.text = sawmillDescription.text;
        if (type == "Field")
            buildingDescriptionText.text = fieldDescription.text;
    }

    public void BuildingLeave()
    {
        buildingDescriptionText.text = defaultDescription.text;
    }
}
