print("====================================")
print("        Lua Calculator")
print("====================================")
print("Type 'exit' or 'quit' to exit")
print("Type 'clear' to clear the result")
print()

local result = nil
local operator = nil
local lastResult = nil

while true do
    io.write("calc> ")
    local input = io.read()
    
    if input == nil or input == "" then
        goto continue
    end
    
    input = input:lower()
    
    if input == "exit" or input == "quit" then
        print("Exiting calculator...")
        break
    elseif input == "clear" then
        result = nil
        operator = nil
        lastResult = nil
        print("Calculator cleared.")
        goto continue
    elseif input == "help" then
        print("Available commands:")
        print("  + : Addition")
        print("  - : Subtraction")
        print("  * : Multiplication")
        print("  / : Division")
        print("  ^ : Power")
        print("  % : Modulo")
        print("  clear : Clear calculator")
        print("  exit : Exit calculator")
        goto continue
    end
    
    local num = tonumber(input)
    
    if num ~= nil then
        if operator == nil then
            result = num
            lastResult = num
            print("Result: " .. result)
        else
            local calcResult = nil
            if operator == "+" then
                calcResult = result + num
            elseif operator == "-" then
                calcResult = result - num
            elseif operator == "*" then
                calcResult = result * num
            elseif operator == "/" then
                if num == 0 then
                    print("Error: Division by zero!")
                else
                    calcResult = result / num
                end
            elseif operator == "^" then
                calcResult = result ^ num
            elseif operator == "%" then
                if num == 0 then
                    print("Error: Modulo by zero!")
                else
                    calcResult = result % num
                end
            end
            
            if calcResult ~= nil then
                result = calcResult
                lastResult = calcResult
                print("Result: " .. result)
            end
            
            operator = nil
        end
    else
        if input == "+" or input == "-" or input == "*" or input == "/" or input == "^" or input == "%" then
            operator = input
            print("Operator: " .. operator)
        else
            print("Invalid input: " .. input)
            print("Type 'help' for available commands")
        end
    end
    
    ::continue::
end

print()
print("Final result: " .. (lastResult or "No calculation"))
print("Goodbye!")