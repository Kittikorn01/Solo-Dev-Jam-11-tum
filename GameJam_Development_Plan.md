# Game Jam Development Plan: "Everything Has a Cost"

เนื่องจากเป็นการทำโปรเจกต์สำหรับ Game Jam ซึ่งมีเวลาจำกัด การวางแผนจึงแบ่งเป็น **"Phases"** หรือระยะการทำงาน เพื่อให้สามารถโฟกัสกับส่วนที่สำคัญที่สุด (Core Mechanics) ก่อน แล้วจึงค่อยเพิ่มรายละเอียด (Polish/Juice) ในภายหลังครับ

---

## 🛠 Technical Constraints (Polish & Efficiency)
*   **Procedural Animation:** ห้ามใช้ Frame-by-frame animation ให้ใช้ Sine Wave (`sin(time)`) ควบคุม Y-scale (Squash & Stretch) แทน
*   **Randomized Decorations:** ใช้ Arena เดียว แต่สุ่ม Spawn ของตกแต่ง (Rocks, Dirt) พร้อมสุ่มหมุนและ Flip เพื่อลดความจำเจ
*   **Cheaty AI:** เพิ่ม Angular Offset ในการเคลื่อนที่ของศัตรู เพื่อไม่ให้ศัตรูเดินซ้อนกันเป็นเส้นตรง (Conga line)
*   **Composition Scaling:** ไม่เพิ่ม HP/Damage ของศัตรู แต่จะเพิ่มความยากผ่านการผสม Wave และจำนวนศัตรูแทน

---

## Phase 1: การตั้งค่าเริ่มต้น & การเคลื่อนไหวพื้นฐาน (Core Movement)
**เป้าหมาย:** สร้างโครงสร้างโปรเจกต์และทำให้ตัวละครขยับได้
*   [ ] **Project Setup:** สร้างโปรเจกต์ Unity 2D (ตั้งค่าโฟลเดอร์ Scripts, Prefabs, Sprites, Sounds)
*   [ ] **Player Placeholder:** สร้างตัวละครชั่วคราว (ใช้รูปทรงเรขาคณิต)
*   [ ] **Top-Down Movement:** เขียนสคริปต์บังคับการเดินแบบ 8 ทิศทาง
*   [ ] **Procedural Animation (Sine Wave):** ใช้ Sine Function ควบคุม Y-axis scale สำหรับ Squash & Stretch และการลอยตัวของอาวุธ (Hovering)

## Phase 2: ระบบการต่อสู้หลัก & กฎของเกม (Combat & Mechanics)
**เป้าหมาย:** ผู้เล่นสามารถโจมตีได้และระบบ "ทางเลือก" ต้องทำงาน
*   [ ] **Melee Attack:** สร้างระบบคลิกเพื่อโจมตีระยะใกล้
*   [ ] **Weapon Visuals:** ทำให้สไปรต์อาวุธหมุนกวาด (Rotation) และมี Sine Wave Hover effect
*   [ ] **Micro-dash:** เพิ่มแรงกระแทกเบาๆ ให้ตัวละครพุ่งไปข้างหน้าเล็กน้อยตอนกดโจมตี
*   [ ] **ระบบ "Everything Has a Cost":**
    *   สาย 1 (โจมตีแรง/เสียเลือด): ดาเมจสูง + เลือดลดเมื่อโจมตีโดน
    *   สาย 2 (โจมตีเร็ว/เวลาลดไว): ความเร็วสูง + เวลานับถอยหลังเร็วขึ้น
*   [ ] **Health & Drop System:** ระบบ HP และการสุ่มดรอป "ขวดยาฟื้นเลือด"

## Phase 3: ศัตรู & AI (Enemies & AI)
**เป้าหมาย:** สร้างศัตรูทั้ง 2 ประเภทให้สมบูรณ์
*   [ ] **Enemy 1 - Chaser (ทรงสามเหลี่ยม):**
    *   **Cheaty AI:** เดินไล่ตามผู้เล่นพร้อมสุ่ม Angular Offset เพื่อให้ศัตรูกระจายตัว (Wandering/Drifting)
    *   **Movement Animation:** ใช้ Sine Wave Squash & Stretch
*   [ ] **Enemy 2 - Boomer (ทรงสี่เหลี่ยม/แปดเหลี่ยม):**
    *   **Pre-explosion:** ใช้ Multiplied Sine Waves เพื่อให้ตัวละครสั่น/เต้นพัลส์ (Pulsate) อย่างรวดเร็วก่อนระเบิด
    *   **The Blast:** สร้าง Particle ระเบิดและทำดาเมจวงกว้าง
*   [ ] **Enemy Death:** ระบบตัวแตกกระจายเป็นเศษ Particle เรขาคณิต

## Phase 4: ระบบด่าน & Game Loop (Level Design & Waves)
**เป้าหมาย:** ทำให้เกมเล่นได้จนจบเกม (ชนะ/แพ้)
*   [ ] **Arena Setup:** ลานประลองเดี่ยว (Single Arena)
*   [ ] **Randomized Decorations:** เขียนสคริปต์สุ่ม Spawn ของตกแต่งพื้น (Rocks, Dirt) 2-3 แบบ พร้อมสุ่ม Rotation (0-360) และ X-Flip
*   [ ] **Wave Spawner (Composition over Stats):**
    *   ไม่เพิ่ม HP/Damage แต่เพิ่มความยากด้วยจำนวนและการผสมประเภทศัตรู (Chaser + Boomer)
    *   **Wave 1:** Chaser จำนวนน้อย
    *   **Wave 2:** Chaser + Boomer
    *   **Wave 3:** Massive Swarm (Climax)
*   [ ] **Game Over / Victory Condition:** เงื่อนไขการจบเกม (เลือดหมด/เวลาหมด/เคลียร์ครบ Wave)

## Phase 5: ความสมบูรณ์แบบ & อารมณ์ร่วม (Juice, Audio & Polish)
**เป้าหมาย:** ใส่รายละเอียดที่ทำให้เกมน่าเล่นและสะใจ (Game Feel)
*   [ ] **Camera Shake:** จอสั่นเมื่อโดนโจมตี หรือเมื่อ Boomer ระเบิด
*   [ ] **Hit Stop (Freeze Frame):** หยุดเวลาเสี้ยววินาทีเมื่อเกิดการระเบิดใหญ่
*   [ ] **Audio System:** BGM เร้าอารมณ์ และ SFX สำหรับ Action ต่างๆ

## Phase 6: Playtest, Balance & Build
**เป้าหมาย:** ตรวจสอบความเรียบร้อยและพร้อมส่ง
*   [ ] **Balancing:** ปรับจูนความยากผ่านจำนวนศัตรูในแต่ละ Wave
*   [ ] **Bug Fixing:** แก้ไขบัคทั่วไป
*   [ ] **Build:** Export เป็น WebGL

---
**💡 คำแนะนำ:** โฟกัสที่ **Procedural Animation** และ **Cheaty AI** เพื่อช่วยประหยัดเวลาทำ Asset และทำให้ AI ดูฉลาดขึ้นโดยใช้ Code เพียงเล็กน้อยครับ
