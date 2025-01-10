from json import dump, loads
import os
import sys
from rich import print
import re
from os import listdir
os.system("cls")
variablesFunctions = ["Character"]



def compile(fileText: str):
    lines = fileText.split("\n")
    variables = {}
    currentLine = 0
    blockComment = False
    ActionLines = []
    for line in lines:
        currentLine+=1
        line = re.sub(r'^\s+', '', line)
        textLine = re.match(r'(\"[^\"]+\") (\w+)(?: (.+))?', line)
        actionLine = re.match(r"\-(\w+)\((.*?)\)", line)
        if line.startswith("*/") or line.endswith("*/"):
            blockComment = False
            continue
        elif line == "" or line.startswith("//") or blockComment:
            continue
        elif line.startswith("/*"):
            blockComment = True
            continue
        # TODO: add that can change vars that are already created
        elif line.startswith("let"):
            result = re.match(r"let (\w+) = (.+);", line)
            if result:
                variable, value = result.groups()
                result = re.match(r"(\w+)\((.*?)\)", value)
                
                if result:
                    function, Args = ExtractFunction(variables, currentLine, result)
                    if function not in variablesFunctions:
                        raise Exception(f"Function {function} is not defined in the list of functions at line {currentLine}")       
                    variables[variable] = {function: Args}
                else:
                    opt = re.search(r"[\+\-\*/]", value)
                    if opt:
                        variables[variable] = eval(value)
                    elif value == "null" or value == "undefined":
                        variables[variable] = None
                    else:
                        variables[variable] = loads(value)
                         
            else:
                raise SyntaxError(f"Invalid syntax at line {currentLine}")
        elif textLine:
            text, character, speed = textLine.groups()
            
            if not speed: speed = 0.02
            else: speed = float(speed)
            characterFunction= variables.get(character)
            if not characterFunction:
                raise KeyError(f"character was not found, error line {currentLine}")
            ActionLines.append({
                "type": "text",
                "text": text,
                "character":characterFunction["Character"]["name"],
                "speed":speed
            })
        elif actionLine:
            function, args = ExtractFunction(variables, currentLine , actionLine, True)
            if function == "createCharacter":
                ActionLines.append({
                "type": "action",
                "action": function,
                "character": args["Character"]["Character"]["name"],
                "posX":args["posX"]
            })
            elif function == "deleteCharacter":
                ActionLines.append({
                "type": "action",
                "action": function,
                "character": args["Character"]["Character"]["name"]
            })
            elif function == "changeEmotion":
                ActionLines.append({
                "type": "action",
                "action": function,
                "character": args["Character"]["Character"]["name"],
                "emotion": args["emotion"],
            })
            elif function == "changeBackground":
                ActionLines.append({
                "type": "action",
                "action": function,
                "background": args["background"],
            })
            elif function == "playSound":
                ActionLines.append({
                "type": "action",
                "action": function,
                "sound": args["sound"],
            })
            else:
                raise TypeError(f"{function} isn't defined, line {currentLine}")  
    return ActionLines

def ExtractFunction(variables: dict, currentLine: int, result: re.Match[str], allowDict: bool = False):
    function, args = result.groups()
    args = args.split(",")
    args = {arg.split("=")[0]: arg.split("=")[1] for arg in args}
    newArgs = {}
    for argKey, argValue in args.items():
        argValue= re.sub(r'^\s+', '', argValue)
        argKey= re.sub(r'^\s+', '', argKey)
        quote= re.match(r'^".*"$', argValue)
        if quote:
            newArgs[argKey] = argValue.strip("\"")
        elif argValue.startswith("\"") or argValue.endswith("\""):
            raise ValueError(f"Invalid Value at line {currentLine}: Unmatched quotes in '{argValue}'")
        elif argValue in variables:
            if isinstance(variables.get(argValue), dict) and not allowDict:
                raise TypeError(f"You can't put Functions as Values like {argValue} at {currentLine}")
            else:
                newArgs[argKey]= variables.get(argValue)
        else:
            newArgs[argKey]= loads(argValue)
    return function,newArgs
    # return {"dialogues": dialogue}

def main():
    # Check if a file path was provided
    if len(sys.argv) < 2:
        for file in listdir("Assets/Scripts/Dialogue Texts"):
            if file.endswith(".mahiro"):
                input_path = f"Assets/Scripts/Dialogue Texts/{file}"  # Fixed path
                output_path = f"Assets/StreamingAssets/Dialogues/{file.replace('.mahiro', '.json')}"

                with open(input_path, "r") as f:
                    data = compile(f.read())
                    with open(output_path, "w") as f:
                        dump(data, f)
        return
    
    # Get the file path from the command line arguments
    file_path = sys.argv[1]

    # Check if the file exists
    if not os.path.isfile(file_path):
        print(f"Error: File '{file_path}' does not exist.")
        return

    # Read and process the file (example for text files)
    try:
        with open(file_path, 'r') as file:
            content = file.read()
    except Exception as e:
        print(f"Error reading the file: {e}")
    print(compile(content))
    
if __name__ == "__main__":
    main()