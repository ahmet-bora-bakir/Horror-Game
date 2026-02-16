bubble :: [Int] -> [Int]
bubble [] = []
bubble [x] = [x]
bubble(x:y:zs)
 |x <= y            = x:bubble (y:zs)             
 |x > y       = y:bubble (x:zs)
 

bubbleCheck :: [Int] -> Bool
bubbleCheck (x:y:zs)
 |zs == []   = True
 |x <= y     = True  && bubbleCheck (y:zs)
 |otherwise = False


main :: IO()
main = do
    print(bubble [4,3,2,1])
    