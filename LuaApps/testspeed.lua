local loop = 100000000
os.timerstart()
for i = 1, loop do
    -- Nothing
end
cost = os.timerstop()
print("Loop " .. loop .. ", cost " .. cost .. "s.")